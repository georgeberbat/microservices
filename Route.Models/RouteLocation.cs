using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Route.Models;

public class RouteLocation
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Route))]
    [Column("route_id")]
    public Guid RouteId { get; set; }

    [Required]
    [ForeignKey("Location")]
    [Column("location_id")]
    public Guid LocationId { get; set; }

    [Required]
    [Column("distance")]
    public double Weight { get; set; }

    public Route Route { get; set; }
    public Location.Models.Location Location { get; set; }
}