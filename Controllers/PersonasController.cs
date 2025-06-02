using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using MiProyectoBackend.Models;
using MiProyectoBackend.Interfaces;

namespace MiProyectoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación para todos los endpoints
    public class PersonaController : ControllerBase
    {
        private readonly IPersonaService _personaService;

        public PersonaController(IPersonaService personaService)
        {
            _personaService = personaService;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var persona = _personaService.GetById(id);
                if (persona == null) 
                    return NotFound(new { message = "Persona no encontrada" });

                var result = new 
                {
                    id = persona.Id,
                    nombre = persona.Nombre,
                    apellido = persona.Apellido,
                    email = persona.Email,
                    rol = persona.Rol?.Nombre  // Devuelve solo el nombre del rol
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAll")]

        public IActionResult GetAll()
        {
            try
            {
                var personas = _personaService.GetAll().Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    apellido = p.Apellido,
                    email = p.Email,
                    rol = p.Rol?.Nombre
                });

                return Ok(personas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")] // Solo admins pueden crear usuarios
        [AllowAnonymous]
        public IActionResult Create([FromBody] PersonaCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var persona = new Persona
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    RolId = dto.RolId != Guid.Empty ? dto.RolId : GetDefaultRolId()
                };

                var createdPersona = _personaService.Create(persona, dto.Password);

                var result = new
                {
                    id = createdPersona.Id,
                    nombre = createdPersona.Nombre,
                    apellido = createdPersona.Apellido,
                    email = createdPersona.Email,
                    rol = createdPersona.Rol?.Nombre
                };

                return CreatedAtAction(nameof(GetById), new { id = createdPersona.Id }, result);
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

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] PersonaUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var persona = new Persona
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    RolId = dto.RolId
                };

                _personaService.Update(id, persona, dto.Password);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo admins pueden eliminar usuarios
        public IActionResult Delete(Guid id)
        {
            try
            {
                _personaService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // Método para obtener el RolId por defecto (ejemplo, puedes adaptarlo)
        private Guid GetDefaultRolId()
        {
            // Aquí deberías consultar tu base de datos o configuración para el rol "User"
            // Por ahora, un Guid fijo de ejemplo (reemplazar por el real)
            return Guid.Parse("00000000-0000-0000-0000-000000000001"); 
        }
    }
}
