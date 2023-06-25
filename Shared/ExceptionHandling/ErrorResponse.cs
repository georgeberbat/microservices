using System.Collections;

namespace Shared.ExceptionHandling
{
    public record ErrorResponse
    {
        public string Message { get; set; }
        public string? StackTrace { get; set; }
        public string Type { get; set; }
        public IDictionary Data { get; set; }
    }
}