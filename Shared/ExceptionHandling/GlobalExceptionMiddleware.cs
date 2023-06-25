using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Exceptions;

namespace Shared.ExceptionHandling;

public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const int Status499ClientClosedRequest = 499; // Client Closed Request

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

#pragma warning disable CA1801
        public async Task Invoke(HttpContext context, IOptions<JsonOptions> options)
#pragma warning restore CA1801
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var innerExceptions = exception.GetInnerExceptions().ToArray();
                var response = new ErrorResponse
                {
                    Message = string.Join(Environment.NewLine, innerExceptions.Select(x => x.Message)),
                    // т.к. типы могут быть Generic
                    Type = exception.GetType().Name.Replace("`1", string.Empty, StringComparison.Ordinal),
                    Data = exception.Data
                };

                var result = new ContentResult
                {
                    StatusCode = ResolveHttpStatusCode(exception),
                    ContentType = "application/json",
                };

                if (result.StatusCode == 500 || exception is ArgumentException)
                {
                    var isStackTrace = exception is not (NotImplementedException or NotSupportedException or OperationCanceledException);
                    response.StackTrace = isStackTrace ? string.Join(Environment.NewLine, innerExceptions.Select(x => x.StackTrace)) : null;
                    _logger.LogError(exception, "Request failed");
                }
                else
                {
                    _logger.LogWarning(exception, "Request complete with warning. {MiddlewareResponse}", response);
                }

                try
                {
                    context.Response.StatusCode = result.StatusCode.Value;
                    context.Response.ContentType = result.ContentType;

                    await context.Response.WriteAsync(response.ToFriendlyJson());
                }
                catch (ObjectDisposedException)
                {
                    // ignore
                }
            }
        }

        private static int ResolveHttpStatusCode(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException _ => StatusCodes.Status403Forbidden,

                DbUpdateConcurrencyException _ => StatusCodes.Status409Conflict,
                AlreadyExistException _ => StatusCodes.Status409Conflict,

                NotFoundException _ => StatusCodes.Status404NotFound,

                ArgumentException _ => StatusCodes.Status400BadRequest,
                UserException _ => StatusCodes.Status400BadRequest,
                ValidationException _ => StatusCodes.Status400BadRequest,

                RetryLimitExceededException _ => StatusCodes.Status408RequestTimeout,
                TimeoutException _ => StatusCodes.Status408RequestTimeout,
                OperationCanceledException _ => Status499ClientClosedRequest,

                RpcException x => x.StatusCode switch
                {
                    StatusCode.Unavailable => StatusCodes.Status408RequestTimeout,
                    StatusCode.Cancelled => Status499ClientClosedRequest,
                    StatusCode.NotFound => StatusCodes.Status404NotFound,
                    StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
                    StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                    StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                },

                _ => 500
            };
        }
    }