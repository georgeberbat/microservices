using System;
using Shared.Exceptions;

namespace Shared.Dal.Exceptions
{
    /// <summary>
    /// Исключение при попытке изменить неизменяемые системные объекты.
    /// </summary>
    /// <remarks>Status: 400</remarks>
    public class EntityChangeRestrictException<T> : UserException
    {
        public EntityChangeRestrictException()
        {
        }

        public EntityChangeRestrictException(string? message) : base(message)
        {
            Data["Type"] = typeof(T).Name;
        }

        public EntityChangeRestrictException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
