namespace ProfileDomain;

public interface IRegisterUserCommand
{
    /// <summary>
    /// Номер телефона
    /// </summary>
    string Phone { get; }

    /// <summary>
    /// Пароль
    /// </summary>
    string Password { get; }
}