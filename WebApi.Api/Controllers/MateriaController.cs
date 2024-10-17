using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Authorize]
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
        public ActionResult<IEnumerable<MateriaEntities>> GetAll()
        {
            var materias = _IMateriaService.GetALL();
            if (materias == null)
            {
                return NotFound();
            }
            return Ok(materias);

        }

        [HttpGet("{id}")]
        public ActionResult<MateriaEntities> GetByID(int id)
        {
            var materia = _IMateriaService.GetByID(id);
            if (materia == null) return NotFound();
            return Ok(materia);
        }

        [HttpPost]
        public ActionResult Add(MateriaEntities materia)
        {
            _IMateriaService.Add(materia);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, MateriaEntities materia)
        {
            var find = _IMateriaService.GetByID(id);
            if (find == null) return NotFound();
            materia.Id = id;
            _IMateriaService.Update(materia);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var find = _IMateriaService.GetByID(id);
            if (find == null) return NotFound();
            _IMateriaService.Delete(id);
            return Ok();
        }
    }
}
