using System;

namespace Shared.Password
{
    public static class PasswordGeneratorExtension
    {
        public static string MakeHashWithGuidId(this IPasswordGenerator generator, Guid salt, string password)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            return generator.MakeHash(salt.ToString("N"), password);
        }
    }
}