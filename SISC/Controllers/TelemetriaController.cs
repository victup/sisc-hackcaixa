using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Telemetria;
using SISC.Models.Enums;
using SISC.Models.Errors;
using SISC.Services.Telemetria;
using SISC.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/telemetria")]
    [Produces("application/json")]
    public class TelemetriaController : ControllerBase
    {
        private readonly ITelemetriaService _telemetria;

        public TelemetriaController(ITelemetriaService telemetria)
        {
            _telemetria = telemetria;
        }

        /// <summary>
        /// Obtém o resumo de eventos de telemetria registrados no sistema.
        /// </summary>
        /// <returns>Resumo com estatísticas e métricas da telemetria</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Obter telemetria",
            Description = "Retorna um resumo com estatísticas e métricas de telemetria registradas no sistema."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Resumo de telemetria retornado com sucesso", typeof(TelemetriaResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Requisição inválida", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum dado de telemetria encontrado", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro inesperado no servidor", typeof(ErrorResponse))]
        public ActionResult<TelemetriaResponse> Obter()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var resumo = _telemetria.ObterEventos();
                _telemetria.Registrar(FuncionalidadeEnum.TL_001_TELEMETRIA.GetDescription(), stopwatch.ElapsedMilliseconds, true);
                return Ok(resumo);
            }
            catch
            {
                stopwatch.Stop();
                _telemetria.Registrar(FuncionalidadeEnum.TL_001_TELEMETRIA.GetDescription(), stopwatch.ElapsedMilliseconds, false);

                throw;
            }
        }
    }
}
