// using AutoMapper;
// using Google.Protobuf.WellKnownTypes;
// using Grpc.Core;
// using Tariff.Services;
//
// namespace Tariff.Api;
//
// public class TariffGrpcService : TariffGrpc.TariffGrpcBase
// {
//     private readonly ITariffService _TariffService;
//     private readonly IMapper _mapper;
//
//     public TariffGrpcService(ITariffService TariffService, IMapper mapper)
//     {
//         _TariffService = TariffService ?? throw new ArgumentNullException(nameof(TariffService));
//         _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//     }
//
//     public override async Task<GetTariffsGrpcResponse> GetTariffs(Empty request, ServerCallContext context)
//     {
//         var Tariffs = await _TariffService.Get(context.CancellationToken);
//         return new GetTariffsGrpcResponse
//         {
//             Tariffs = { Tariffs.Select(x => _mapper.Map<TariffGrpcModel>(x)) }
//         };
//     }
//
//     public override async Task<GetTariffsGrpcResponse> SearchByName(SearchByNameGrpcRequest request,
//         ServerCallContext context)
//     {
//         var Tariffs = (await _TariffService.SearchByName(request.Substring, context.CancellationToken)).ToArray();
//         return new GetTariffsGrpcResponse
//         {
//             Tariffs = { Tariffs.Select(x => _mapper.Map<TariffGrpcModel>(x)) }
//         };
//     }
//
//     public override async Task<GetTariffsGrpcResponse> SearchByAddress(SearchByAddressGrpcRequest request,
//         ServerCallContext context)
//     {
//         var Tariffs = (await _TariffService.SearchByAddress(request.Substring, context.CancellationToken))
//             .ToArray();
//         return new GetTariffsGrpcResponse
//         {
//             Tariffs = { Tariffs.Select(x => _mapper.Map<TariffGrpcModel>(x)) }
//         };
//     }
//
//     public override async Task<GetTariffsGrpcResponse> SearchByArea(SearchByAreaGrpcRequest request,
//         ServerCallContext context)
//     {
//         var Tariffs = await _TariffService.SearchByArea(
//             request.MinLatitude, request.MaxLatitude, request.MinLongitude, request.MaxLongitude,
//             context.CancellationToken);
//
//         return new GetTariffsGrpcResponse
//         {
//             Tariffs = { Tariffs.Select(x => _mapper.Map<TariffGrpcModel>(x)) }
//         };
//     }
//
//     public override async Task<Empty> CreateTariff(CreateTariffGrpcRequest request, ServerCallContext context)
//     { 
//         await _TariffService.Create(_mapper.Map<Models.Tariff>(request.Tariff), context.CancellationToken);
//         return new Empty();
//     }
//
//     public override async Task<Empty> UpdateTariff(UpdateTariffGrpcRequest request, ServerCallContext context)
//     {
//         await _TariffService.Update(_mapper.Map<Models.Tariff>(request.Tariff), context.CancellationToken);
//         return new Empty();
//     }
//
//     public override async Task<Empty> DeleteTariff(DeleteTariffGrpcRequest request, ServerCallContext context)
//     {
//         await _TariffService.Delete(Guid.Parse(request.Id), context.CancellationToken);
//         return new Empty();
//     }
// }