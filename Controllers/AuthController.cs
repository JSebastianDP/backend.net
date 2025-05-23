using Microsoft.AspNetCore.Mvc;
using MiProyectoBackend.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        var user = _authService.Authenticate(loginDto.Email, loginDto.Password);

        if (user == null)
            return Unauthorized("Usuario o contraseña incorrectos");

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto registerDto)
    {
        var user = new Persona
        {
            Id = Guid.NewGuid(),
            Nombre = registerDto.Nombre,
            Apellido = registerDto.Apellido,
            Email = registerDto.Email,
            Rol = registerDto.Rol
        };

        var createdUser = _authService.Register(user, registerDto.Password);
        return Ok(createdUser);
    }
}
