using System;
using Dex.Ef.Contracts.Entities;

namespace ProfileDomain;

public class Notification : IEntity<Guid>, ICreatedUtc
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }

    public DateTime CreatedUtc { get; set; }
    
    public string Text { get; set; }

    public string Title { get; set; }
    
    public bool Viewed { get; set; }
    
    public virtual User? User { get; set; }
}