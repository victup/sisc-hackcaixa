using AutoMapper;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Options;
using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;
using SISC.Exceptions;
using SISC.Models.Produto;
using SISC.Models.Simulacao;
using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;
using SISC.Settings;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SISC.Services.Simulacao
{
    public class SimulacaoService : ISimulacaoService
    {
        private readonly ISimulacaoRepository _simulacaoRepo;
        private readonly IProdutoRepository _produtoRepo;
        private readonly EventHubProducerClient _eventHubProducer;
        private readonly IMapper _mapper;
        private readonly ILogger<SimulacaoService> _logger;

        public SimulacaoService(
            ISimulacaoRepository simulacaoRepo, 
            IProdutoRepository produtoRepo,
            IOptions<EventHubSettings> options,
            ILogger<SimulacaoService> logger,
            IMapper mapper)
        {
            _simulacaoRepo = simulacaoRepo;
            _produtoRepo = produtoRepo;
            _mapper = mapper;
            _logger = logger;

            var settings = options.Value;
            _eventHubProducer = new EventHubProducerClient(settings.ConnectionString);
        }

        public async Task<SimulacaoCreateResponse> CriarSimulacaoAsync(SimulacaoRequest request)
        {
            var produto = await SelecionarProdutoCompativelAsync(request);

            var simulacao = CriarSimulacao(request, produto);

            await _simulacaoRepo.AddAsync(simulacao);

            await EnviarSimulacaoParaEventHubAsync(simulacao);

            return _mapper.Map<SimulacaoCreateResponse>(simulacao);
        }

        private async Task<Produto> SelecionarProdutoCompativelAsync(SimulacaoRequest request)
        {
            var produtos = await _produtoRepo.GetAllAsync();

            var produtosCompativeis = produtos.Where(p =>
                request.ValorDesejado >= p.VrMinimo &&
                request.ValorDesejado <= p.VrMaximo &&
                request.Prazo >= p.NuMinimoMeses &&
                request.Prazo <= p.NuMaximoMeses);

            if (!produtosCompativeis.Any())
                throw new NegocioException("Nenhum produto atende os parâmetros da simulação.");

            return produtosCompativeis
                .OrderBy(p => p.PcTaxaJuros)
                .First();
        }

        private Models.Simulacao.Simulacao CriarSimulacao(SimulacaoRequest request, Produto produto)
        {
            var resultadoSac = CalcularSac(request.ValorDesejado, request.Prazo, produto.PcTaxaJuros);
            var resultadoPrice = CalcularPrice(request.ValorDesejado, request.Prazo, produto.PcTaxaJuros);

            return new Models.Simulacao.Simulacao
            {
                CodigoProduto = produto.CoProduto,
                DescricaoProduto = produto.NoProduto,
                TaxaJuros = produto.PcTaxaJuros,
                ValorDesejado = request.ValorDesejado,
                Prazo = request.Prazo,
                DataCriacao = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")),
                Resultados = new List<ResultadoSimulacao>
        {
            new ResultadoSimulacao { Tipo = "SAC", Parcelas = resultadoSac },
            new ResultadoSimulacao { Tipo = "PRICE", Parcelas = resultadoPrice }
        }
            };
        }

        private async Task EnviarSimulacaoParaEventHubAsync(Models.Simulacao.Simulacao simulacao)
        {
            try
            {
                var json = JsonSerializer.Serialize(simulacao, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                using EventDataBatch eventBatch = await _eventHubProducer.CreateBatchAsync();

                if (!eventBatch.TryAdd(new EventData(System.Text.Encoding.UTF8.GetBytes(json))))
                    throw new NegocioException("Envelope muito grande para enviar ao EventHub.");

                await _eventHubProducer.SendAsync(eventBatch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar simulação {Id} para EventHub", simulacao.Id);
            }
        }

        public async Task<SimulacaoGetAllResponse> ObterTodasAsync(int pagina = 1, int qtdPorPagina = 10)
        {
            var sims = await _simulacaoRepo.GetAllAsync();

            var registrosPaginados = PaginarSimulacoes(sims, pagina, qtdPorPagina);

            return CriarRespostaPaginada(sims.Count(), registrosPaginados, pagina);
        }

        private List<ResultadoSimulacaoGetAllResponse> PaginarSimulacoes(IEnumerable<Models.Simulacao.Simulacao> sims, int pagina, int qtdPorPagina)
        {
            return sims
                .Skip((pagina - 1) * qtdPorPagina)
                .Take(qtdPorPagina)
                .Select(s => _mapper.Map<ResultadoSimulacaoGetAllResponse>(s))
                .ToList();
        }

        private SimulacaoGetAllResponse CriarRespostaPaginada(int totalRegistros, List<ResultadoSimulacaoGetAllResponse> registros, int pagina)
        {
            return new SimulacaoGetAllResponse
            {
                pagina = pagina,
                qtdRegistros = totalRegistros,
                qtdRegistrosPagina = registros.Count,
                registros = registros
            };
        }

        public async Task<SimulacaoByDiaResponse> ObterPorDataAsync(DateTime data)
        {
            var sims = await _simulacaoRepo.GetByDateAsync(data);

            var agrupado = sims
                .GroupBy(s => new ProdutoKey(s.CodigoProduto, s.DescricaoProduto))
                .Select(g => CriarResultado(g))
                .ToList();

            return new SimulacaoByDiaResponse
            {
                DataReferencia = data.Date,
                Simulacoes = agrupado
            };
        }

        private ResultadoSimulacaoByDiaResponse CriarResultado(IGrouping<ProdutoKey, Models.Simulacao.Simulacao> grupo)
        {
            return new ResultadoSimulacaoByDiaResponse
            {
                codigoProduto = grupo.Key.CodigoProduto,
                descricaoProduto = grupo.Key.DescricaoProduto,
                taxaMediaJuro = CalcularTaxaMedia(grupo),
                valorMedioPrestacao = CalcularValorMedioPrestacao(grupo),
                valorTotalDesejado = CalcularValorTotalDesejado(grupo),
                valorTotalCredito = CalcularValorTotalCredito(grupo)
            };
        }

        private decimal CalcularTaxaMedia(IEnumerable<Models.Simulacao.Simulacao> grupo)
        {
            return grupo.Average(x => x.TaxaJuros);
        }

        private decimal CalcularValorMedioPrestacao(IEnumerable<Models.Simulacao.Simulacao> grupo)
        {
            return grupo.Average(x =>
                x.Resultados
                    .SelectMany(r => r.Parcelas)
                    .Average(p => p.ValorPrestacao)
            );
        }

        private decimal CalcularValorTotalDesejado(IEnumerable<Models.Simulacao.Simulacao> grupo)
        {
            return grupo.Sum(x => x.ValorDesejado);
        }

        private decimal CalcularValorTotalCredito(IEnumerable<Models.Simulacao.Simulacao> grupo)
        {
            return grupo.Sum(x =>
                x.Resultados
                    .SelectMany(r => r.Parcelas)
                    .Sum(p => p.ValorPrestacao)
            );
        }

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

        private class ProdutoKey
        {
            public int CodigoProduto { get; }
            public string DescricaoProduto { get; }

            public ProdutoKey(int codigoProduto, string descricaoProduto)
            {
                CodigoProduto = codigoProduto;
                DescricaoProduto = descricaoProduto;
            }
        }
    }
}
