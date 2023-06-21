using ProfileDomain;

// ReSharper disable once CheckNamespace
public partial class RegisterUserRequest : IRegisterUserCommand
{
    public static readonly RegisterUserRequest Empty = new();
}