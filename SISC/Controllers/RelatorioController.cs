using Microsoft.AspNetCore.Mvc;
using SISC.DTOs.Relatorio;
using SISC.Models.Errors;
using SISC.Services.Relatorio;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/relatorio")]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatorioController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
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
            var relatorio = await _relatorioService.GerarRelatorioAsync();
            return Ok(relatorio);
        }
    }
}
