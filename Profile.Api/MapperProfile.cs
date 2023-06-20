namespace Profile;

internal class MapperProfile : AutoMapper.Profile
{
    public MapperProfile()
    {
        CreateMap<ProfileDomain.User, User>()
            .ForMember(x => x.Sub, expression => expression.MapFrom(x => x.Id))
            .ForMember(x => x.Name, expression => expression.MapFrom(x => x.Phone.ToString()))
            .ForMember(x => x.IsActive, expression => expression.MapFrom(x => x.DeletedUtc == null))
            .ForAllOtherMembers(expression => expression.Ignore());
    }
}