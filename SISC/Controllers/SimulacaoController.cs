using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISC.Data;
using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.Models.Produto;

namespace SISC.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SimulacaoController : ControllerBase
    {
        private readonly ProdutoDbContext _produtoDb;

        public SimulacaoController(ProdutoDbContext produtoDb)
        {
            _produtoDb = produtoDb;
        }

        [HttpGet("produtos")]
        [ProducesResponseType(typeof(IEnumerable<Produto>), 200)]
        public async Task<IActionResult> GetProdutos()
        {
            var produtos = await _produtoDb.Produtos.ToListAsync();
            return Ok(produtos);
        }

        [HttpPost("criar")]
        [ProducesResponseType(typeof(SimulacaoResponse), 200)]
        [ProducesResponseType(400)]
        public IActionResult CriarSimulacao([FromBody] SimulacaoRequest request)
        {
            if (request == null || request.ValorDesejado <= 0)
                return BadRequest("Requisição inválida");

            var simulacao = new SimulacaoResponse
            {
                IdSimulacao = DateTime.UtcNow.Ticks,
                CodigoProduto = request.CodigoProduto,
                DescricaoProduto = "Produto Teste",
                ValorDesejado = request.ValorDesejado,
                Prazo = request.Prazo,
                TaxaJuros = 10.5m,
                Tipo = "Exemplo",
                Parcelas = new List<ParcelaResponse>
                {
                    new ParcelaResponse
                    {
                        Numero = 1,
                        ValorAmortizacao = 100,
                        ValorJuros = 10,
                        ValorPrestacao = 110
                    }
                }
            };

            return Ok(simulacao);
        }
    }
}