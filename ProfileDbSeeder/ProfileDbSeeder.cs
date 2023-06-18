using System;
using System.Linq;
using System.Threading.Tasks;
using FakeData.Profile;
using Profile.Dal;
using ProfileDomain;
using Shared.BaseDbSeeder.Seeder;
using Shared.Dal.Seeder;
using Shared.Password;

namespace ProfileDbSeeder
{
    public class ProfileDbSeeder : BaseEFSeeder<ProfileDbContext>, IDbSeeder
    {
        private readonly IPasswordGenerator _pwdGen;

        public ProfileDbSeeder(IPasswordGenerator passwordGenerator)
        {
            _pwdGen = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
        }

        protected override async Task EnsureSeedData(ProfileDbContext dbContext)
        {
            if (!dbContext.Users.Any())
            {
                var userId = UserConstantId.FakeIdArray[0];
                await dbContext.Users.AddAsync(new User
                {
                    Id = userId,
                    Phone = "37377943964",
                    Password = _pwdGen.MakeHash(userId.ToString("N"), "132565QWEq"),
                    LastName = "Berbat",
                    FirstName = "Georgy",
                }).ConfigureAwait(false);

                userId = UserConstantId.FakeIdArray[1];
                await dbContext.Users.AddAsync(new User
                {
                    Id = userId,
                    Email = "invariantcalibration@gmail.com",
                    Phone = "79540000003",
                    Password = _pwdGen.MakeHash(userId.ToString("N"), "132565QWEq"),
                    LastName = "Ivanov",
                    FirstName = "Ivan",
                }).ConfigureAwait(false);

                await dbContext.SaveChangesAsync();
            }
        }

        public Task RunAsync(bool ensureDeleted)
        {
            throw new NotImplementedException();
        }
    }
}