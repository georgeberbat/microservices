namespace ProfileDomain;

public interface IRegisterUserCommand
{
    /// <summary>
    /// Номер телефона
    /// </summary>
    string PhoneNumber { get; }

    /// <summary>
    /// Пароль
    /// </summary>
    string Password { get; }
}