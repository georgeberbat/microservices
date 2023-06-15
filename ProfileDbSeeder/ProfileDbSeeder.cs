using System;
using System.Linq;
using System.Threading.Tasks;
using FakeData.Profile;
using Microsoft.Extensions.Internal;
using Profile.Dal;
using Profile.Dal.Model;
using Shared.BaseDbSeeder.Seeder;
using Shared.Password;

namespace ProfileDbSeeder
{
    public class ProfileDbSeeder : BaseEFSeeder<ProfileDbContext>, IDbSeeder
    {
        private readonly ISystemClock _systemClock;
        private readonly IPasswordGenerator _pwdGen;

        public ProfileDbSeeder(ISystemClock systemClock, IPasswordGenerator passwordGenerator, ProfileDbContext dbContext)
            : base(dbContext)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _pwdGen = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
        }

        protected override async Task EnsureSeedData()
        {
            if (!_dbContext.Users.Any())
            {
                var userId = UserConstantId.FakeIdArray[0];
                await _dbContext.Users.AddAsync(new UserDb
                {
                    Id = userId,
                    Phone = "37377943964",
                    Password = _pwdGen.MakeHash(userId.ToString("N"), "132565QWEq"),
                    LastName = "Berbat",
                    FirstName = "Georgy",
                }).ConfigureAwait(false);

                userId = UserConstantId.FakeIdArray[1];
                await _dbContext.Users.AddAsync(new UserDb
                {
                    Id = userId,
                    Email = "invariantcalibration@gmail.com",
                    Phone = "79540000003",
                    Password = _pwdGen.MakeHash(userId.ToString("N"), "132565QWEq"),
                    LastName = "Ivanov",
                    FirstName = "Ivan",
                }).ConfigureAwait(false);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}