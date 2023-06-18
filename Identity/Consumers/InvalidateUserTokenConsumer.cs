using System;
using System.Threading.Tasks;
using Identity.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Commands;
using Shared.MassTransit;

namespace Identity.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal sealed class InvalidateUserTokenConsumer : BaseConsumer<UserTokenCommand>
    {
        private readonly IInvalidateUserTokenService _invalidateUserToken;

        public InvalidateUserTokenConsumer(IInvalidateUserTokenService invalidateUserToken, ILogger<InvalidateUserTokenConsumer> logger) : base(logger)
        {
            _invalidateUserToken = invalidateUserToken ?? throw new ArgumentNullException(nameof(invalidateUserToken));
        }

        protected override async Task Process(ConsumeContext<UserTokenCommand> context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var userIds = context.Message.UserIds;

            await _invalidateUserToken.InvalidateToken(userIds, context.CancellationToken);
        }
    }
}