using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dex.Extensions;
using IdentityModel;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shared.Extensions;
using Shared.Logger;
using Shared.Options;
using Shared.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shared
{

    public abstract class BaseStartup
    {
        public const string ApplicationName = "Application template 1.0";
        private const string AllowOriginConfigName = "AllowSpecificOrigin";

        private readonly Assembly _entryAssembly = Assembly.GetEntryAssembly()
                                                   ?? throw new InvalidOperationException("GetEntryAssembly is null");

        /// <summary>
        /// Будет проинициализирован только после вызова Configure
        /// </summary>
        private IServiceProvider? Sp { get; set; }

        private AuthorizationSettings AuthorizationSettings =>
            Sp?.GetRequiredService<IOptions<AuthorizationSettings>>().Value
            ?? throw new InvalidOperationException(
                "Can't get AuthorizationSettings, please InitServiceProvider() from Configure()");

        protected IWebHostEnvironment Environment { get; }
        protected IConfiguration Configuration { get; }

        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // services
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            ConfigureLocalization(services);
            var mvcBuilder = services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // swagger
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(ConfigureSwagger);


            // authentication
            services.AddOptionsWithDataAnnotationsValidation<AuthorizationSettings>(
                Configuration.GetSection(nameof(AuthorizationSettings)));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options => ConfigureJwt(AuthorizationSettings, options))
                .AddOAuth2Introspection("introspection",
                    options => ConfigureIntrospection(AuthorizationSettings,
                        options)); //https://www.rfc-editor.org/rfc/rfc7662

            // authorization
            services.AddAuthorization(options => ConfigureAuthorization(AuthorizationSettings, options));

            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IPrincipal),
                sp => sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ??
                      new GenericPrincipal(Identity.Anonymous, Array.Empty<string>()));

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(AllowOriginConfigName, builder =>
                    {
                        if (Environment.IsDevelopment())
                        {
                            builder.AllowAnyOrigin();
                        }
                        else
                        {
                            var cors = Configuration["CorsSpecificOrigins"];
                            if (!cors.IsNullOrEmpty())
                            {
                                builder.WithOrigins(cors.Split(',', ';').Select(x => x.Trim()).ToArray());
                            }
                        }

                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            });

            // automapper
            services.AddAutoMapper(expression => expression.AddMaps(_entryAssembly));
        }

        private static void ConfigureLocalization(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ru")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "ru");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddLocalization(x => x.ResourcesPath = "Resources");
        }

        protected virtual void ConfigureSwagger(SwaggerGenOptions options)
        {
            options.EnableAnnotations();

            options.TagActionsBy(
                api =>
                {
                    if (api.GroupName != null && !Regex.IsMatch(api.GroupName, @"^\d"))
                    {
                        return new[] {api.GroupName};
                    }

                    if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return new[] {controllerActionDescriptor.ControllerName};
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
        }

        private static void ConfigureJwt(AuthorizationSettings authorizationSettings, JwtBearerOptions options)
        {
            // options.MetadataAddress = "https://<base.url>/identity/.well-known/openid-configuration";
            options.Authority = authorizationSettings.AuthorityUrl.ToString();
            options.Audience = authorizationSettings.ApiResource;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = 1.Minutes()
            };

            // если токен не содержит точку (это референс токен), уходим на introspection endpoint
            options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
        }

        private static void ConfigureIntrospection(AuthorizationSettings authorizationSettings,
            OAuth2IntrospectionOptions options)
        {
            options.Authority = authorizationSettings.AuthorityUrl.ToString();
            options.EnableCaching = authorizationSettings.IntrospectionCacheTimeSeconds > 0;
            options.CacheDuration = TimeSpan.FromSeconds(authorizationSettings.IntrospectionCacheTimeSeconds);
            options.ClientId = authorizationSettings.ApiResource;
            options.ClientSecret = authorizationSettings.ApiResourceSecret;
            options.SaveToken = true;
        }

        protected virtual void ConfigureAuthorization(AuthorizationSettings authorizationSettings,
            AuthorizationOptions options)
        {
            if (authorizationSettings == null) throw new ArgumentNullException(nameof(authorizationSettings));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!authorizationSettings.ApiScopeRequired.IsNullOrEmpty())
            {
                options.AddPolicy("ApiScopeRequired", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", authorizationSettings.ApiScopeRequired);
                    Log.Information("Scope policy added - {PolicyName}", "ApiScopeRequired");
                });
            }

            if (!authorizationSettings.ApiScopeOnlyRequired.IsNullOrEmpty())
            {
                foreach (var scope in authorizationSettings.ApiScopeOnlyRequired)
                {
                    var policyName = "only-" + scope;
                    options.AddPolicy(policyName, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", scope);
                    });
                    Log.Information("Scope policy added - {PolicyName}", policyName);
                }
            }

            if (!authorizationSettings.ApiPolicies.IsNullOrEmpty())
            {
                const string claimType = "policy";

                foreach (var apiPolicy in authorizationSettings.ApiPolicies)
                {
                    // fullAccess должен игнорироваться
                    if (apiPolicy == "fullAccess")
                        continue;

                    options.AddPolicy(apiPolicy, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim(claimType);

                        //write включает read
                        if (apiPolicy.EndsWith(".read", StringComparison.InvariantCulture))
                        {
                            var basePolicy =
                                apiPolicy.Replace(".read", string.Empty, StringComparison.InvariantCulture);
                            policy.RequireAssertion(context =>
                            {
                                var policyValue = context.User.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
                                return policyValue != null &&
                                       (policyValue.Contains(apiPolicy, StringComparison.InvariantCulture) ||
                                        policyValue.Contains($"{basePolicy}.write",
                                            StringComparison.InvariantCulture) ||
                                        policyValue.Contains("fullAccess", StringComparison.InvariantCulture));
                            });
                        }
                        else
                        {
                            policy.RequireAssertion(context =>
                            {
                                var policyValue = context.User.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
                                return policyValue != null &&
                                       (policyValue.Contains(apiPolicy, StringComparison.InvariantCulture) ||
                                        policyValue.Contains("fullAccess", StringComparison.InvariantCulture));
                            });
                        }
                    });

                    Log.Information("Policy added - {PolicyName}", apiPolicy);
                }
            }
        }

        // configuration
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (env == null) throw new ArgumentNullException(nameof(env));

            InitServiceProvider(app.ApplicationServices);

            // request logger
            app.UseRequestLogger();

            // process exceptions
            // app.UseMiddleware<GlobalExceptionMiddleware>();

            //localization
            var localizationOptions =
                app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);

            // check automapper config
            var provider = app.ApplicationServices.GetRequiredService<IConfigurationProvider>();
            provider.AssertConfigurationIsValid();

            app.UseRouting();
            app.UseCors(AllowOriginConfigName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(builder =>
            {
                ConfigureEndpoints(builder);
                ConfigureSystemEndpoints(builder);
            });

            if (Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

        protected void InitServiceProvider(IServiceProvider serviceProvider)
        {
            Sp = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected static void ConfigureSystemEndpoints(IEndpointRouteBuilder builder)
        {
            builder.MapHealthChecks("/health", new HealthCheckOptions
            {
                AllowCachingResponses = true,
                ResponseWriter = HealthReportResponseWriter,
                ResultStatusCodes = new Dictionary<HealthStatus, int>()
                {
                    {HealthStatus.Healthy, StatusCodes.Status200OK},
                    {HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable},
                    {HealthStatus.Degraded, StatusCodes.Status200OK},
                }
            });

            builder.MapGet("/ver",
                    context => context.Response.WriteAsync(
                        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"))
                .AllowAnonymous();
        }

        protected virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        private static async Task HealthReportResponseWriter(HttpContext context, HealthReport report)
        {
            var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {Converters = {new JsonStringEnumConverter()}};
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    report.Status,
                    report.TotalDuration.TotalSeconds,
                    Entities = report.Entries.Select(x => new
                    {
                        x.Key,
                        x.Value.Status
                    })
                },
                jsonSerializerOptions));
        }
    }
}