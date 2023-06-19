using Dex.Specifications;
using ProfileDomain;

namespace Profile.Dal.Specifications
{
    public class ActiveUserSpecification : Specification<User>
    {
        public ActiveUserSpecification() : base(db => !db.DeletedUtc.HasValue)
        {
        }
    }
}