using AutoMapper;

namespace Location.Api.Mapping;

public class Main : Profile
{
    public Main()
    {
        CreateMap<LocationGrpcModel, Models.Location>()
            .ForMember(x => x.Id, x => x.MapFrom(y => Guid.Parse(y.Id)));

        CreateMap<Models.Location, LocationGrpcModel>()
            .ForMember(x => x.Id, x => x.MapFrom(y => y.Id.ToString()));
    }
}