using Dex.Specifications;
using ProfileDomain;

namespace Profile.Dal.Specifications
{
    public class ActiveUserByPhoneSpecification : AndSpecification<User>
    {
        public ActiveUserByPhoneSpecification(string phone)
            : base(new Specification<User>(db => db.Phone == phone), new ActiveUserSpecification())
        {
        }
    }
}