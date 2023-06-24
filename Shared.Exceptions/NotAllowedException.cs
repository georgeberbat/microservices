namespace Shared.Exceptions;

/// <remarks>Status: 403</remarks>
public class NotAllowedException : Exception
{
    public NotAllowedException() : base("The user does not have permission for this action.")
    {
    }

    public NotAllowedException(string? message) : base(message)
    {
    }

    public NotAllowedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}