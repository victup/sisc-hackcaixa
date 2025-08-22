using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Telemetria;
using SISC.Services.Telemetria;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/telemetria")]
    public class TelemetriaController : ControllerBase
    {
        private readonly ITelemetriaService _telemetria;

        public TelemetriaController(ITelemetriaService telemetria)
        {
            _telemetria = telemetria;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TelemetriaResponse), StatusCodes.Status200OK)]
        public ActionResult<TelemetriaResponse> Obter()
        {
            var resumo = _telemetria.ObterEventos();
            return Ok(resumo);
        }
    }
}
