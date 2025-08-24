using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;
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
        [ProducesResponseType(typeof(SimulacaoCreateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SimulacaoCreateResponse>> Criar([FromBody] SimulacaoRequest request)
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
        [ProducesResponseType(typeof(SimulacaoGetAllResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SimulacaoGetAllResponse>> Todas(
    [FromQuery] int pagina = 1,
    [FromQuery] int qtdPorPagina = 10)
        {
            if (pagina <= 0 || qtdPorPagina <= 0)
                return BadRequest("Os parâmetros de paginação devem ser maiores que zero.");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var sims = await _service.ObterTodasAsync(pagina, qtdPorPagina);
                stopwatch.Stop();

                _telemetria.Registrar("Simulacao.Todas", stopwatch.ElapsedMilliseconds, true);

                return Ok(sims);
            }
            catch
            {
                stopwatch.Stop();

                _telemetria.Registrar("Simulacao.Todas", stopwatch.ElapsedMilliseconds, false);
                throw;
            }
        }

        [HttpGet("por-data/{data:datetime}")]
        [ProducesResponseType(typeof(SimulacaoByDiaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SimulacaoByDiaResponse>> PorData(DateTime data)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var sims = await _service.ObterPorDataAsync(data);
                stopwatch.Stop();

                _telemetria.Registrar("Simulacao.PorData", stopwatch.ElapsedMilliseconds, true);

                return Ok(sims);
            }
            catch
            {
                stopwatch.Stop();

                _telemetria.Registrar("Simulacao.PorData", stopwatch.ElapsedMilliseconds, false);
                throw;
            }
        }

    }
}