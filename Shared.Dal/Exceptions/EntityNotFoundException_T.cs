using System;

namespace Shared.Dal.Exceptions
{
    /// <summary>
    /// Исключение при невозможности обнаружить объект по переданным параметрам.
    /// </summary>
    /// <remarks>Status: 404</remarks>
    public class EntityNotFoundException<T> : EntityNotFoundException
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string key) : base(key, typeof(T).Name)
        {
        }

        public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}