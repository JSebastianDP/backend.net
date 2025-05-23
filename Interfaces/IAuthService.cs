using MiProyectoBackend.Models;
public interface IAuthService
{
    Persona Authenticate(string email, string password);
    string GenerateJwtToken(Persona user);
    Persona Register(Persona user, string password);
}
