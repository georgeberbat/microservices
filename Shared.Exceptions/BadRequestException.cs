using Shared.Extensions;

namespace Shared.Exceptions
{
    /// <remarks>Status: 400</remarks>
    public class BadRequestException : UserException
    {
        public BadRequestException(params (string, object)[] args)
            : base($"The specified parameters had an invalid value: {string.Join(",", args.Select(x => $"{x.Item1} : {x.Item2.ToFriendlyJson()}"))}")
        {
        }

        public BadRequestException(string? message) : base(message)
        {
        }

        public BadRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}