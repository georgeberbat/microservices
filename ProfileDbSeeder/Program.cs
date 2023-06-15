using System.Threading.Tasks;
using Shared.BaseDbSeeder;

namespace ProfileDbSeeder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await SeedRunner.Seed<Startup, ProfileDbSeeder>(args);
            // await SeedRunner.Seed<Startup, SecurityTokenDbSeeder>(args);
        }
    }
}