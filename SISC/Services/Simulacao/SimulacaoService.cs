using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;
using SISC.Models.Simulacao;
using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;

namespace SISC.Services.Simulacao
{
    public class SimulacaoService : ISimulacaoService
    {
        private readonly ISimulacaoRepository _simulacaoRepo;
        private readonly IProdutoRepository _produtoRepo;

        public SimulacaoService(ISimulacaoRepository simulacaoRepo, IProdutoRepository produtoRepo)
        {
            _simulacaoRepo = simulacaoRepo;
            _produtoRepo = produtoRepo;
        }

        public async Task<SimulacaoCreateResponse> CriarSimulacaoAsync(SimulacaoRequest request)
        {
            var produtos = await _produtoRepo.GetAllAsync();
            var produto = produtos.FirstOrDefault(p =>
                request.ValorDesejado >= p.VrMinimo &&
                request.ValorDesejado <= p.VrMaximo &&
                request.Prazo >= p.NuMinimoMeses &&
                request.Prazo <= p.NuMaximoMeses);

            if (produto == null)
                throw new ArgumentException("Nenhum produto atende os parâmetros da simulação.");

            var resultadoSac = CalcularSac(request.ValorDesejado, request.Prazo, produto.PcTaxaJuros);
            var resultadoPrice = CalcularPrice(request.ValorDesejado, request.Prazo, produto.PcTaxaJuros);

            var simulacao = new SISC.Models.Simulacao.Simulacao
            {
                CodigoProduto = produto.CoProduto,
                DescricaoProduto = produto.NoProduto,
                TaxaJuros = produto.PcTaxaJuros,
                ValorDesejado = request.ValorDesejado,
                Prazo = request.Prazo,
                DataCriacao = DateTime.UtcNow,
                Resultados = new List<ResultadoSimulacao>
            {
                new ResultadoSimulacao
                {
                    Tipo = "SAC",
                    Parcelas = resultadoSac
                },
                new ResultadoSimulacao
                {
                    Tipo = "PRICE",
                    Parcelas = resultadoPrice
                }
            }
            };

            await _simulacaoRepo.AddAsync(simulacao);

            return MapToResponseCreate(simulacao);
        }

        public async Task<SimulacaoGetAllResponse> ObterTodasAsync(int pagina = 1, int qtdPorPagina = 10)
        {
            var sims = await _simulacaoRepo.GetAllAsync();

            var totalRegistros = sims.Count();
            var registrosPaginados = sims
                .Skip((pagina - 1) * qtdPorPagina)
                .Take(qtdPorPagina)
                .Select(MapToResultadoResumido)
                .ToList();

            return new SimulacaoGetAllResponse
            {
                pagina = pagina,
                qtdRegistros = totalRegistros,
                qtdRegistrosPagina = registrosPaginados.Count,
                registros = registrosPaginados
            };
        }

        public async Task<IEnumerable<SimulacaoCreateResponse>> ObterPorDataAsync(DateTime data)
        {
            var sims = await _simulacaoRepo.GetByDateAsync(data);
            return sims.Select(MapToResponseCreate);
        }

        // --- MÉTODOS DE CÁLCULO ---

        private List<Parcela> CalcularSac(decimal valor, int prazo, decimal taxa)
        {
            var parcelas = new List<Parcela>();
            var amortizacao = valor / prazo;

            for (int i = 1; i <= prazo; i++)
            {
                var saldoDevedor = valor - (amortizacao * (i - 1));
                var juros = saldoDevedor * taxa;
                var prestacao = amortizacao + juros;

                parcelas.Add(new Parcela
                {
                    Numero = i,
                    ValorAmortizacao = Math.Round(amortizacao, 2),
                    ValorJuros = Math.Round(juros, 2),
                    ValorPrestacao = Math.Round(prestacao, 2)
                });
            }

            return parcelas;
        }

        private List<Parcela> CalcularPrice(decimal valor, int prazo, decimal taxa)
        {
            var parcelas = new List<Parcela>();
            var fator = (decimal)(Math.Pow((double)(1 + taxa), prazo) - 1) /
                        (taxa * (decimal)Math.Pow((double)(1 + taxa), prazo));
            var prestacaoConstante = valor / fator;

            var saldoDevedor = valor;
            for (int i = 1; i <= prazo; i++)
            {
                var juros = saldoDevedor * taxa;
                var amortizacao = prestacaoConstante - juros;
                saldoDevedor -= amortizacao;

                parcelas.Add(new Parcela
                {
                    Numero = i,
                    ValorAmortizacao = Math.Round(amortizacao, 2),
                    ValorJuros = Math.Round(juros, 2),
                    ValorPrestacao = Math.Round(prestacaoConstante, 2)
                });
            }

            return parcelas;
        }

        private SimulacaoCreateResponse MapToResponseCreate(SISC.Models.Simulacao.Simulacao s)
        {
            return new SimulacaoCreateResponse
            {
                IdSimulacao = s.Id,
                CodigoProduto = s.CodigoProduto,
                DescricaoProduto = s.DescricaoProduto,
                TaxaJuros = s.TaxaJuros,
                ResultadoSimulacao = s.Resultados.Select(r => new ResultadoSimulacaoResponse
                {
                    Tipo = r.Tipo,
                    Parcelas = r.Parcelas.Select(p => new ParcelaResponse
                    {
                        Numero = p.Numero,
                        ValorAmortizacao = p.ValorAmortizacao,
                        ValorJuros = p.ValorJuros,
                        ValorPrestacao = p.ValorPrestacao
                    }).ToList()
                }).ToList()
            };
        }

        private ResultadoSimulacaoResumidoResponse MapToResultadoResumido(SISC.Models.Simulacao.Simulacao s)
        {
            return new ResultadoSimulacaoResumidoResponse
            {
                IdSimulacao = s.Id,
                valorDesejado = s.ValorDesejado,
                Prazo = s.Prazo,
                valorTotalParcelas = s.Resultados
                    .SelectMany(r => r.Parcelas)
                    .Sum(p => p.ValorPrestacao)
            };
        }
    }

}
