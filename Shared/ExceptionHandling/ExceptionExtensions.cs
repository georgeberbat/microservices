using System;
using System.Collections.Generic;

namespace Shared.ExceptionHandling;

public static class ExceptionExtensions
{
    public static IEnumerable<Exception> GetInnerExceptions(this Exception exception, int count = 5)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var innerException = exception;
        do
        {
            yield return innerException;

            innerException = innerException.InnerException;
            count--;
        } while (innerException != null && count > 0);
    }
}