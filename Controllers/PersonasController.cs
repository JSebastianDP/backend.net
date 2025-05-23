using Microsoft.AspNetCore.Mvc;
using System;
using MiProyectoBackend.Models;
using MiProyectoBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MiProyectoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var persona = _personaService.GetById(id);
            if (persona == null) return NotFound();
            return Ok(persona);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_personaService.GetAll());
        }

        [HttpPost]
        public IActionResult Create([FromBody] PersonaCreateDto dto)
        {
            try
            {
                var persona = new Persona
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    Rol = dto.Rol
                };
                var createdPersona = _personaService.Create(persona, dto.Password);
                return Ok(createdPersona);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] PersonaUpdateDto dto)
        {
            try
            {
                var persona = new Persona
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    Rol = dto.Rol
                };
                _personaService.Update(id, persona, dto.Password);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _personaService.Delete(id);
            return NoContent();
        }
    }
}
