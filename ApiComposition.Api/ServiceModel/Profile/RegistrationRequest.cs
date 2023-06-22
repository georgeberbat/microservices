using ApiComposition.Models;

namespace ApiComposition.Api.ServiceModel;

public class RegistrationRequest : IRegistrationRequest
{
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;
}