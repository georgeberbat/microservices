namespace Location.Models.Commands;

public class OnLocationRemovedCommand
{
    public Guid LocationId { get; set; }
    public string Name { get; init; } = null!;
}