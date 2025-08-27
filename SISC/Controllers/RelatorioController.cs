using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Relatorio;
using SISC.Models.Enums;
using SISC.Models.Errors;
using SISC.Services.Relatorio;
using SISC.Services.Telemetria;
using SISC.Utils;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/relatorio")]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;
        private readonly ITelemetriaService _telemetria;

        public RelatorioController(IRelatorioService relatorioService, ITelemetriaService telemetria)
        {
            _relatorioService = relatorioService;
            _telemetria = telemetria;
        }

        /// <summary>
        /// Gera relatório consolidado de produtos e simulações com insights da IA.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(RelatorioResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<RelatorioResponse>> Get()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var relatorio = await _relatorioService.GerarRelatorioAsync();
                _telemetria.Registrar(FuncionalidadeEnum.RL_001_REL_IA.GetDescription(), stopwatch.ElapsedMilliseconds, true);
                return Ok(relatorio);
            }
            catch
            {
                stopwatch.Stop();
                _telemetria.Registrar(FuncionalidadeEnum.RL_001_REL_IA.GetDescription(), stopwatch.ElapsedMilliseconds, false);

                throw;
            }
            
        }
    }
}
