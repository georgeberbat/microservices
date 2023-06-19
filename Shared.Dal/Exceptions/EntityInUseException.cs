using System;
using Shared.Exceptions;

namespace Shared.Dal.Exceptions
{
    public class EntityInUserException<T> : UserException
    {
        /// <summary>
        /// Эксепшен указывающий на то, что объект используется
        /// и требуемое действие над ним (обычно удаление) - невозможно
        /// </summary>
        /// <remarks>Status: 400</remarks>
        public EntityInUserException(string? message) : base(message)
        {
            Data["Type"] = typeof(T).Name;
        }

        public EntityInUserException()
        {
        }

        public EntityInUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}