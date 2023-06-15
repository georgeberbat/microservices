using Profile.Dal.Model;
using ProfileDomain;

namespace Profile.Mapping
{
    public class Main : AutoMapper.Profile
    {
        public Main()
        {
            CreateMap<User, UserDb>()
                .ForMember(x => x.CreatedUtc, y => y.Ignore())
                .ForMember(x => x.DeletedUtc, y => y.Ignore())
                .ForMember(x => x.UpdatedUtc, y => y.Ignore())
                .ForMember(x => x.Password, y => y.Ignore())
                .ForMember(x => x.EmailConfirmed, y => y.Ignore());
            
            CreateMap<UserDb, User>();
        }
    }
}