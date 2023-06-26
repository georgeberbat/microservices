using System;
using Dex.Cap.Outbox.Interfaces;

namespace ProfileDomain.Commands;

public class RouteChangedCommand : IOutboxMessage
{
    public Guid UserId { get; set; }
    public Guid RouteId { get; set; }
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;

    public Guid MessageId { get; init; }
}