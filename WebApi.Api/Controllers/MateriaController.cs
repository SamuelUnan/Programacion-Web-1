using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaService _IMateriaService;

        public MateriaController(IMateriaService materiaService)
        {
            _IMateriaService = materiaService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            ResponseEntities<List<MateriaEntities>> _Response = new ResponseEntities<List<MateriaEntities>>();

            try
            {
                _Response.StatusCode = "00";
                _Response.Message = "Success";
                _Response.Result = _IMateriaService.GetAll();
                return Ok(_Response);
            }
            catch (Exception ex)
            {
                _Response.StatusCode = "05";
                _Response.Message = ex.Message;
                return NotFound(_Response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            ResponseEntities<MateriaEntities> _Response = new ResponseEntities<MateriaEntities>();

            try
            {
                _Response.StatusCode = "00";
                _Response.Message = "Success";
                _Response.Result = await _IMateriaService.GetById(id);

                return Ok(_Response);

            }
            catch (Exception ex)
            {
                _Response.StatusCode = "05";
                _Response.Message = ex.Message;
                return NotFound(_Response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(MateriaEntities materia)
        {
            ResponseEntities<MateriaEntities> _Response = new ResponseEntities<MateriaEntities>();
            try
            {
                _Response.StatusCode = "00";
                _Response.Message = "Success";
                _Response.Result = await _IMateriaService.Add(materia);

                return Ok(_Response);

            }
            catch (Exception ex)
            {
                _Response.StatusCode = "05";
                _Response.Message = ex.Message;
                return NotFound(_Response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(MateriaEntities materia)
        {
            ResponseEntities<MateriaEntities> _Response = new ResponseEntities<MateriaEntities>();

            try
            {
                _Response.StatusCode = "00";
                _Response.Message = "Success";
                _Response.Result = await _IMateriaService.Update(materia);

                return Ok(_Response);

            }
            catch (Exception ex)
            {
                _Response.StatusCode = "05";
                _Response.Message = ex.Message;
                return NotFound(_Response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseEntities<int> _Response = new ResponseEntities<int>();

            try
            {
                _Response.StatusCode = "00";
                _Response.Message = "Success";
                _Response.Result = await _IMateriaService.Delete(id);

                return Ok(_Response);

            }
            catch (Exception ex)
            {
                _Response.StatusCode = "05";
                _Response.Message = ex.Message;
                return NotFound(_Response);
            }
        }
    }
}
