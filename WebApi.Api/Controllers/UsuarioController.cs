using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("Registro")]
    public async Task<IActionResult> Register([FromBody] UsuarioDto usuario)
    {
        var user = new UsuarioEntities
        {
            Username = usuario.Username,
            Rol = "User"
        };

        try
        {
            await _usuarioService.Registrar(user, usuario.Password);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("Autenticar")]
    public async Task<IActionResult> Authenticate([FromBody] UsuarioDto Usuario)
    {
        var user = await _usuarioService.Autenticar(Usuario.Username, Usuario.Password);

        if (user == null)
            return BadRequest(new { message = "Datos Incorrectos" });

        var token = _usuarioService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _usuarioService.GetAll();
        return Ok(users);
    }
    }
}
