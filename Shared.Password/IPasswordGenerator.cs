namespace Shared.Password
{
    public interface IPasswordGenerator
    {
        string MakeHash(string salt, string password);
    }
}