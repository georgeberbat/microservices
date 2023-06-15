using Microsoft.AspNetCore.Mvc;

namespace Location.Api;

public class LocationController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllLocations()
    {
        // Implementation: Retrieve all locations from the database
        // and return them as an ActionResult<IEnumerable<Location>>.
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public IActionResult GetLocationById(Guid id)
    {
        // Implementation: Retrieve the location with the specified id
        // from the database and return it as an ActionResult<Location>.
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult CreateLocation(Models.Location location)
    {
        // Implementation: Create a new location in the database using the
        // provided location object, and return the created location
        // as an ActionResult<Location>.
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateLocation(Guid id, Models.Location location)
    {
        // Implementation: Update the location with the specified id in the
        // database using the provided location object.
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteLocation(Guid id)
    {
        // Implementation: Delete the location with the specified id from
        // the database.
        throw new NotImplementedException();
    }
}