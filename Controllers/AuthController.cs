using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiProyectoBackend.Models;
using System.ComponentModel.DataAnnotations;

namespace MiProyectoBackend.Controllers
{
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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = _authService.Authenticate(loginDto.Email, loginDto.Password);

                if (user == null)
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });

                var token = _authService.GenerateJwtToken(user);
                
                return Ok(new 
                { 
                    token,
                    user = new 
                    {
                        id = user.Id,
                        nombre = user.Nombre,
                        apellido = user.Apellido,
                        email = user.Email,
                        rol = user.Rol?.Nombre // Aquí devuelves el nombre del rol, no el objeto completo
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new Persona
                {
                    Id = Guid.NewGuid(),
                    Nombre = registerDto.Nombre,
                    Apellido = registerDto.Apellido,
                    Email = registerDto.Email,
                    RolId = registerDto.RolId != Guid.Empty ? registerDto.RolId : GetDefaultRolId()
                };

                var createdUser = _authService.Register(user, registerDto.Password);

                return Ok(new
                {
                    message = "Usuario registrado exitosamente",
                    token = _authService.GenerateJwtToken(createdUser),
                    user = new
                    {
                        id = createdUser.Id,
                        nombre = createdUser.Nombre,
                        apellido = createdUser.Apellido,
                        email = createdUser.Email,
                        rol = createdUser.Rol?.Nombre
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // Método para obtener un RolId por defecto (por ejemplo "User")
        private Guid GetDefaultRolId()
        {
            // Aquí puedes hacer una consulta para traer el Id del rol "User"
            // o usar un valor fijo si sabes cuál es.

            // Ejemplo rápido (debes inyectar el contexto o servicio para hacerlo bien)
            throw new NotImplementedException("Implementa la lógica para obtener el RolId por defecto");
        }
    }
}
