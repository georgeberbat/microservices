using AutoMapper;
using Dex.Extensions;
using Google.Protobuf.WellKnownTypes;
using Shared.Helpers;
using Tariff.Models;


namespace Tariff.Api.Mapping;

public class Main : Profile
{
    public Main()
    {
        CreateMap<Route, Route>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.UserId, x => x.Ignore())
            .ForMember(x => x.CreatedUtc, x => x.Ignore())
            .ForMember(x => x.RouteUnits, x => x.Ignore());

        CreateMap<RouteUnit, RouteUnit>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.CreatedUtc, x => x.Ignore())
            .ForMember(x => x.Route, x => x.Ignore())
            .ForMember(x => x.Tariff, x => x.Ignore())
            .ForMember(x => x.UpdatedUtc, x => x.Ignore());

        CreateMap<Models.Tariff, Models.Tariff>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.UserId, x => x.Ignore())
            .ForMember(x => x.CreatedUtc, x => x.Ignore())
            .ForMember(x => x.DeletedUtc, x => x.Ignore())
            .ForMember(x => x.TariffUnits, x => x.Ignore());

        CreateMap<TariffUnit, TariffUnit>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.CreatedUtc, x => x.Ignore())
            .ForMember(x => x.Tariff, x => x.Ignore())
            .ForMember(x => x.UpdatedUtc, x => x.Ignore());


        CreateMap<CreateRouteRequestGrpc, Route>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Route.Id.IsNullOrEmpty() ? Guid.NewGuid() : Guid.Parse(src.Route.Id)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Route.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Route.Description))
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.RouteUnits, opt => opt.MapFrom(src => src.Route.RouteUnits));

        CreateMap<UpdateRouteRequestGrpc, Route>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => Guid.Parse(src.Route.Id)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Route.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Route.Description))
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.RouteUnits, opt => opt.Ignore());

        CreateMap<Route, RouteGrpc>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedUtc.SetUtcKind())))
            .ForMember(dest => dest.RouteUnits, opt => opt.MapFrom(src => src.RouteUnits));


        CreateMap<RouteUnitGrpc, RouteUnit>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id.IsNullOrEmpty() ? Guid.NewGuid() : Guid.Parse(src.Id)))
            .ForMember(dest => dest.RouteId, opt => opt.MapFrom(src => Guid.Parse(src.ParentId)))
            .ForMember(dest => dest.TariffId, opt => opt.MapFrom(src => Guid.Parse(src.TariffId)))
            .ForMember(dest => dest.CreatedUtc, opt => opt.MapFrom(src => src.CreatedUtc.ToDateTime()))
            .ForMember(dest => dest.UpdatedUtc, opt => opt.MapFrom(src => src.UpdatedUtc.ToDateTime()))
            .ForMember(dest => dest.Route, opt => opt.Ignore())
            .ForMember(dest => dest.Tariff, opt => opt.Ignore());

        CreateMap<RouteUnit, RouteUnitGrpc>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.RouteId.ToString()))
            .ForMember(dest => dest.TariffId, opt => opt.MapFrom(src => src.TariffId.ToString()))
            .ForMember(dest => dest.CreatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedUtc.SetUtcKind())))
            .ForMember(dest => dest.UpdatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.UpdatedUtc.SetUtcKind())));


        CreateMap<CreateTariffRequestGrpc, Models.Tariff>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Tariff.Id.IsNullOrEmpty() ? Guid.NewGuid() : Guid.Parse(src.Tariff.Id)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tariff.Name))
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.TariffUnits, opt => opt.MapFrom(src => src.Tariff.TariffUnits));

        CreateMap<UpdateTariffRequestGrpc, Models.Tariff>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => Guid.Parse(src.Tariff.Id)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tariff.Name))
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.TariffUnits, opt => opt.Ignore());

        CreateMap<Models.Tariff, TariffGrpc>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedUtc.SetUtcKind())))
            .ForMember(dest => dest.UpdatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.UpdatedUtc.SetUtcKind())))
            .ForMember(dest => dest.DeletedUtc,
                opt => opt.Ignore())
            .ForMember(dest => dest.TariffUnits, opt => opt.MapFrom(src => src.TariffUnits));


        CreateMap<TariffUnitGrpc, TariffUnit>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id.IsNullOrEmpty() ? Guid.NewGuid() : Guid.Parse(src.Id)))
            .ForMember(dest => dest.TariffId, opt => opt.MapFrom(src => Guid.Parse(src.ParentId)))
            .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
            .ForMember(dest => dest.NextLocationId, opt => opt.MapFrom(src => src.NextLocationId))
            .ForMember(dest => dest.WeightScaleCoefficient, opt => opt.MapFrom(src => src.WeightScaleCoefficient))
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.Tariff, opt => opt.Ignore());

        CreateMap<TariffUnit, TariffUnitGrpc>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.TariffId.ToString()))
            .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
            .ForMember(dest => dest.NextLocationId, opt => opt.MapFrom(src => src.NextLocationId))
            .ForMember(dest => dest.WeightScaleCoefficient, opt => opt.MapFrom(src => src.WeightScaleCoefficient))
            .ForMember(dest => dest.CreatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedUtc.SetUtcKind())))
            .ForMember(dest => dest.UpdatedUtc,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.UpdatedUtc.SetUtcKind())));
    }
}