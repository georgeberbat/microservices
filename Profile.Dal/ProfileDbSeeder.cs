using System;
using System.Linq;
using System.Threading.Tasks;
using FakeData.Profile;
using ProfileDomain;
using Shared.Dal.Seeder;
using Shared.Password;

namespace Profile.Dal
{
    public class ProfileDbSeeder : BaseEFSeeder<ProfileDbContext>
    {
        private readonly IPasswordGenerator _pwdGen;

        public ProfileDbSeeder(string connectionString, IPasswordGenerator passwordGenerator)
        {
            _pwdGen = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
            AddDbContext(connectionString);
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
                    Password = _pwdGen.MakeHash(userId.ToString("N"), "mypass123"),
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
    }
}