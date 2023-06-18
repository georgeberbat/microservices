using System;
using MassTransit;

namespace Shared.Commands
{
    public class UserTokenCommand : IConsumer
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public Guid[] UserIds { get; set; }
    }
}