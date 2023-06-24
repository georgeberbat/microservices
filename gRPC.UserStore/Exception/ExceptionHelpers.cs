using System;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace gRPC.UserStore.Exception;

public static class ExceptionHelpers
{
    public static RpcException Handle<T>(this System.Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId) =>
        exception switch
        {
            TimeoutException timeoutException => HandleTimeoutException(timeoutException, context, logger,
                correlationId),
            RpcException rpcException => HandleRpcException(rpcException, logger, correlationId),
            _ => HandleDefault(exception, context, logger, correlationId)
        };

    private static RpcException HandleTimeoutException<T>(TimeoutException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - A timeout occurred");

        var status = new Status(StatusCode.Internal, "An external resource did not answer within the time limit");

        return new RpcException(status, CreateTrailers(correlationId));
    }

    private static RpcException HandleRpcException<T>(RpcException exception, ILogger<T> logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - An error occurred");
        var trailers = exception.Trailers;
        trailers.Add(CreateTrailers(correlationId)[0]);
        return new RpcException(new Status(exception.StatusCode, exception.Message), trailers);
    }

    private static RpcException HandleDefault<T>(System.Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - An error occurred");

        var trailers = CreateTrailers(correlationId);
        string exceptionType;

        switch (exception)
        {
            case AlreadyExistException:
                context.Status = new Status(StatusCode.AlreadyExists, exception.Message);
                exceptionType = nameof(AlreadyExistException);
                break;

            case UserException:
                context.Status = new Status(StatusCode.InvalidArgument, exception.Message);
                exceptionType = nameof(UserException);
                break;

            case NotAllowedException:
                context.Status = new Status(StatusCode.PermissionDenied, exception.Message);
                exceptionType = nameof(NotAllowedException);
                break;

            case NotFoundException:
                context.Status = new Status(StatusCode.NotFound, exception.Message);
                exceptionType = nameof(NotFoundException);
                break;

            default:
                context.Status = new Status(StatusCode.Internal, exception.Message);
                exceptionType = "UnknownException";
                break;
        }

        trailers.Add("ExceptionMessage", exception.Message);
        trailers.Add("ExceptionType", exceptionType);
        
        return new RpcException(context.Status, CreateTrailers(correlationId));
    }

    /// <summary>
    ///  Adding the correlation to Response Trailers
    /// </summary>
    /// <param name="correlationId"></param>
    /// <returns></returns>
    private static Metadata CreateTrailers(Guid correlationId)
    {
        var trailers = new Metadata();
        trailers.Add("CorrelationId", correlationId.ToString());
        return trailers;
    }
}