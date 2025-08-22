using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.Services.Simulacao;
using SISC.Services.Telemetria;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/simulacoes")]
    public class SimulacaoController : ControllerBase
    {
        private readonly ISimulacaoService _service;
        private readonly ITelemetriaService _telemetria;

        public SimulacaoController(ISimulacaoService service, ITelemetriaService telemetria)
        {
            _service = service;
            _telemetria = telemetria;
        }

        [HttpPost("simular")]
        [ProducesResponseType(typeof(SimulacaoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SimulacaoResponse>> Criar([FromBody] SimulacaoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var simulacao = await _service.CriarSimulacaoAsync(request);

                stopwatch.Stop();
                _telemetria.Registrar("Simulacao.Criar", stopwatch.ElapsedMilliseconds, true);

                return Ok(simulacao);
            }
            catch
            {
                stopwatch.Stop();
                _telemetria.Registrar("Simulacao.Criar", stopwatch.ElapsedMilliseconds, false);

                throw;
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SimulacaoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SimulacaoResponse>>> Todas()
        {
            var sims = await _service.ObterTodasAsync();
            return Ok(sims);
        }

        [HttpGet("por-data/{data:datetime}")]
        [ProducesResponseType(typeof(IEnumerable<SimulacaoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SimulacaoResponse>>> PorData(DateTime data)
        {
            var sims = await _service.ObterPorDataAsync(data);
            return Ok(sims);
        }

    }
}