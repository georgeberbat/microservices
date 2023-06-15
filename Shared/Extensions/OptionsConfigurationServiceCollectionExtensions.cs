using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Extensions
{
    /// <summary>
    /// Extension methods for adding configuration related options services to the DI container.
    /// </summary>
    public static class OptionsConfigurationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a configuration instance which <typeparamref name="TOptions"/> will bind against and validates the instance
        /// on by using <see cref="DataAnnotationValidateOptions{TOptions}"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="validateAtStartup">Indicates if the options should be validated during application startup.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> AddOptionsWithDataAnnotationsValidation<TOptions>(this IServiceCollection services, IConfiguration? config = null, bool validateAtStartup = true) where TOptions : class
        {
            var optionBuilder = config != null ? services.AddOptions<TOptions>().Bind(config) : new OptionsBuilder<TOptions>(services, nameof(TOptions));
            
            services.AddSingleton<IValidateOptions<TOptions>>(new DataAnnotationValidateOptions<TOptions>(Microsoft.Extensions.Options.Options.DefaultName));

            if (validateAtStartup)
            {
                ValidateAtStartup(services, typeof(TOptions));
            }

            return optionBuilder;
        }

        /// <summary>
        /// Registers a type of options to be validated during application startup.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="type">The type of options to validate.</param>
        static void ValidateAtStartup(IServiceCollection services, Type type)
        {
            var existingService = services.Select(x => x.ImplementationInstance).OfType<StartupOptionsValidation>().FirstOrDefault();
            if (existingService == null)
            {
                existingService = new StartupOptionsValidation();
                services.AddSingleton<IStartupFilter>(existingService);
            }

            existingService.OptionsTypes.Add(type);
        }

        /// <summary>
        /// A startup filter that validates option instances during application startup.
        /// </summary>
        class StartupOptionsValidation : IStartupFilter
        {
            private IList<Type>? _optionsTypes;

            /// <summary>
            /// The type of options to validate.
            /// </summary>
            public IList<Type> OptionsTypes => _optionsTypes ??= new List<Type>();

            /// <inheritdoc/>
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    if (_optionsTypes != null)
                    {
                        foreach (var optionsType in _optionsTypes)
                        {
                            var options = builder.ApplicationServices.GetService(typeof(IOptions<>).MakeGenericType(optionsType));
                            if (options != null)
                            {
                                // Retrieve the value to trigger validation
                                var unused = ((IOptions<object>)options).Value;
                            }
                        }
                    }

                    next(builder);
                };
            }
        }
    }
}
