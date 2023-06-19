using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Shared.Dal
{
    public interface IEntityChangesTrigger
    {
        Type EntityType { get; }
        void RunTrigger(IEnumerable<object> entities, EntityState state);
    }
}