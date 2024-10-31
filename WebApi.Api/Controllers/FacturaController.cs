using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            var factura = _facturaService.GetByID(id);
            if (factura == null) { return NotFound(); }
            return Ok(factura);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Factura>> GetAll()
        {
            var facturas = _facturaService.GetALL();
            if (facturas == null) { return NotFound(); }
            return Ok(facturas);
        }

        [HttpPost]
        public ActionResult Add([FromBody] Factura factura)
        {
            if (factura == null)
            {
                return BadRequest("Datos no encontrados");
            }
            _facturaService.Add(factura);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Factura factura)
        {
            var find = _facturaService.GetByID(id);
            if (find == null) return NotFound();
            factura.FacturaId = id;
            _facturaService.Update(factura);
            return Ok();
        }
    }
}
