using AutoMapper;
using Dex.Extensions;
using Google.Protobuf.WellKnownTypes;
using Shared.Helpers;

namespace Location.Api.Mapping;

public class Main : Profile
{
    public Main()
    {
        CreateMap<LocationGrpcModel, Models.Location>()
            .ForMember(x => x.Id, x => x.MapFrom(y =>
                    y.Id.IsNullOrEmpty() ? Guid.NewGuid() : Guid.Parse(y.Id)
                )
            )
            .ForMember(x => x.DeletedUtc, x => x.Ignore())
            .ForMember(x => x.UpdatedUtc, x => x.Ignore())
            .ForMember(x => x.CreatedUtc, x => x.Ignore());

        CreateMap<Models.Location, LocationGrpcModel>()
            .ForMember(x => x.DeletedUtc, x => x.MapFrom(y =>
                y.DeletedUtc.HasValue ? Timestamp.FromDateTime(y.DeletedUtc.Value.SetUtcKind()) : null))
            .ForMember(x => x.UpdatedUtc, x => x.MapFrom(y =>
                Timestamp.FromDateTime(y.UpdatedUtc.SetUtcKind())))
            .ForMember(x => x.CreatedUtc, x => x.MapFrom(y =>
                Timestamp.FromDateTime(y.CreatedUtc.SetUtcKind())))
            .ForMember(x => x.Id, x => x.MapFrom(y => y.Id.ToString()));
    }
}