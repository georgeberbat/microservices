using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;

namespace Shared.MassTransit
{
    public abstract class BaseConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        protected ILogger Logger { get; }

        protected BaseConsumer(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task Consume(ConsumeContext<TMessage> context)
        {
            try
            {
                await Process(context);
            }
            catch (Exception e)
            {
                LogError(context, e);
                throw;
            }
        }

        protected void LogError(ConsumeContext<TMessage> context, Exception e)
        {
            var friendlyJson = context.Message.ToFriendlyJson().ToString();
            Logger.LogError(e, "Consumer process failed. {MessageData}", friendlyJson[..Math.Min(4096, friendlyJson.Length - 1)]);
        }

        protected abstract Task Process(ConsumeContext<TMessage> context);
    }
}