using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Secret = IdentityServer4.Models.Secret;

namespace Identity.Mapping
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<Secret, ApiResourceSecret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));
        }
    }
}