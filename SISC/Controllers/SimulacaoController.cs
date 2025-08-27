using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;
using SISC.Models.Enums;
using SISC.Models.Errors;
using SISC.Services.Simulacao;
using SISC.Services.Telemetria;
using SISC.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/simulacoes")]
    [Produces("application/json")]
    public class SimulacaoController : ControllerBase
    {
        private readonly ISimulacaoService _service;
        private readonly ITelemetriaService _telemetria;

        public SimulacaoController(ISimulacaoService service, ITelemetriaService telemetria)
        {
            _service = service;
            _telemetria = telemetria;
        }

        /// <summary>
        /// Cria uma nova simulação.
        /// </summary>
        [HttpPost("simular")]
        [Consumes("application/json")]
        [SwaggerOperation(
            Summary = "Criar simulação",
            Description = "Cria uma nova simulação a partir dos dados informados."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Simulação criada com sucesso", typeof(SimulacaoCreateResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Requisição inválida", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro inesperado no servidor", typeof(ErrorResponse))]
        public async Task<ActionResult<SimulacaoCreateResponse>> Criar([FromBody] SimulacaoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var simulacao = await _service.CriarSimulacaoAsync(request);

                stopwatch.Stop();
                _telemetria.Registrar(FuncionalidadeEnum.SM_001_SIMULAR.GetDescription(), stopwatch.ElapsedMilliseconds, true);

                return Ok(simulacao);
            }
            catch
            {
                stopwatch.Stop();
                _telemetria.Registrar(FuncionalidadeEnum.SM_001_SIMULAR.GetDescription(), stopwatch.ElapsedMilliseconds, false);

                throw;
            }
        }

        /// <summary>
        /// Lista todas as simulações cadastradas.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar simulações",
            Description = "Retorna todas as simulações de forma paginada."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista de simulações retornada com sucesso", typeof(SimulacaoGetAllResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Parâmetros de paginação inválidos", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro inesperado no servidor", typeof(ErrorResponse))]
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

                _telemetria.Registrar(FuncionalidadeEnum.SM_002_SIMULACOES.GetDescription(), stopwatch.ElapsedMilliseconds, true);

                return Ok(sims);
            }
            catch
            {
                stopwatch.Stop();

                _telemetria.Registrar(FuncionalidadeEnum.SM_002_SIMULACOES.GetDescription(), stopwatch.ElapsedMilliseconds, false);
                throw;
            }
        }

        /// <summary>
        /// Obtém todas as simulações realizadas em uma data específica.
        /// </summary>
        [HttpGet("por-data/{data:datetime}")]
        [SwaggerOperation(
            Summary = "Listar simulações por data",
            Description = "Retorna todas as simulações realizadas em uma data específica."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Simulações retornadas com sucesso", typeof(SimulacaoByDiaResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Data inválida ou mal formatada", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhuma simulação encontrada para a data", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro inesperado no servidor", typeof(ErrorResponse))]
        public async Task<ActionResult<SimulacaoByDiaResponse>> PorData(DateTime data)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var sims = await _service.ObterPorDataAsync(data);
                stopwatch.Stop();

                _telemetria.Registrar(FuncionalidadeEnum.SM_003_SIMULACOES_DATA.GetDescription(), stopwatch.ElapsedMilliseconds, true);

                return Ok(sims);
            }
            catch
            {
                stopwatch.Stop();

                _telemetria.Registrar(FuncionalidadeEnum.SM_003_SIMULACOES_DATA.GetDescription(), stopwatch.ElapsedMilliseconds, false);
                throw;
            }
        }

    }
}