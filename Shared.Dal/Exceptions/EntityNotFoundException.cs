using System;
using Shared.Exceptions;

namespace Shared.Dal.Exceptions
{
    public class EntityNotFoundException : NotFoundException
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string key, string type) : base(key)
        {
            Data["Key"] = key;
            Data["Type"] = type;
        }

        public EntityNotFoundException(string? message) : base(message)
        {
        }

        public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}