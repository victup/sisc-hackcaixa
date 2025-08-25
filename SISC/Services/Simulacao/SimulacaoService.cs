using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Options;
using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;
using SISC.Models.Simulacao;
using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;
using SISC.Settings;

namespace SISC.Services.Simulacao
{
    public class SimulacaoService : ISimulacaoService
    {
        private readonly ISimulacaoRepository _simulacaoRepo;
        private readonly IProdutoRepository _produtoRepo;
        private readonly EventHubProducerClient _eventHubProducer;
        private readonly ILogger<SimulacaoService> _logger;

        public SimulacaoService(
            ISimulacaoRepository simulacaoRepo, 
            IProdutoRepository produtoRepo,
            IOptions<EventHubSettings> options,
            ILogger<SimulacaoService> logger)
        {
            _simulacaoRepo = simulacaoRepo;
            _produtoRepo = produtoRepo;
            _logger = logger;

            var settings = options.Value;
            _eventHubProducer = new EventHubProducerClient(settings.ConnectionString);
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

            try
            {
                var json = JsonSerializer.Serialize(simulacao, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                using EventDataBatch eventBatch = await _eventHubProducer.CreateBatchAsync();
                if (!eventBatch.TryAdd(new EventData(System.Text.Encoding.UTF8.GetBytes(json))))
                    throw new Exception("Envelope muito grande para enviar ao EventHub.");

                await _eventHubProducer.SendAsync(eventBatch);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Falha ao enviar simulação {Id} para EventHub", simulacao.Id);
            }

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

        public async Task<SimulacaoByDiaResponse> ObterPorDataAsync(DateTime data)
        {
            var sims = await _simulacaoRepo.GetByDateAsync(data);

            var agrupado = sims
                .GroupBy(s => new { s.CodigoProduto, s.DescricaoProduto })
                .Select(g => new ResultadoSimulacaoByDiaResponse
                {
                    codigoProduto = g.Key.CodigoProduto,
                    descricaoProduto = g.Key.DescricaoProduto,
                    taxaMediaJuro = g.Average(x => x.TaxaJuros),

                    valorMedioPrestacao = g.Average(x =>
                        x.Resultados
                            .SelectMany(r => r.Parcelas)
                            .Average(p => p.ValorPrestacao)
                    ),

                    valorTotalDesejado = g.Sum(x => x.ValorDesejado),

                    valorTotalCredito = g.Sum(x =>
                        x.Resultados
                         .SelectMany(r => r.Parcelas)
                         .Sum(p => p.ValorPrestacao)
                    )
                })
                .ToList();

            return new SimulacaoByDiaResponse
            {
                DataReferencia = data.Date,
                Simulacoes = agrupado
            };
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
                ResultadoSimulacao = s.Resultados.Select(r => new ResultadoSimulacaoCreateResponse
                {
                    Tipo = r.Tipo,
                    Parcelas = r.Parcelas.Select(p => new ParcelaCreateResponse
                    {
                        Numero = p.Numero,
                        ValorAmortizacao = p.ValorAmortizacao,
                        ValorJuros = p.ValorJuros,
                        ValorPrestacao = p.ValorPrestacao
                    }).ToList()
                }).ToList()
            };
        }

        private ResultadoSimulacaoGetAllResponse MapToResultadoResumido(SISC.Models.Simulacao.Simulacao s)
        {
            return new ResultadoSimulacaoGetAllResponse
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
