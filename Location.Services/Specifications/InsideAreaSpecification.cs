using Dex.Specifications;

namespace Location.Services.Specifications;

public class InsideAreaSpecification : Specification<Models.Location>
{
    public InsideAreaSpecification(double minLatitude, double maxLatitude,
        double minLongitude, double maxLongitude)
        : base(x =>
            x.Latitude <= maxLatitude &&
            x.Latitude >= minLatitude &&
                    x.Longitude <= maxLongitude &&
                    x.Longitude >= minLongitude)
    {
    }
}