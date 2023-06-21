namespace ApiComposition.Models;

public interface IRegistrationRequest
{
    public string Phone { get; set; }
    public string Password { get; set; }
}