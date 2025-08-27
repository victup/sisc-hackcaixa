using SISC.DTOs.Relatorio;
using SISC.Integrations;
using SISC.Models.Produto;
using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;

namespace SISC.Services.Relatorio
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IProdutoRepository _produtoRepo;
        private readonly ISimulacaoRepository _simulacaoRepo;
        private readonly IOpenAiClient _openAi;

        public RelatorioService(IProdutoRepository produtoRepo, ISimulacaoRepository simulacaoRepo, IOpenAiClient openAi)
        {
            _produtoRepo = produtoRepo;
            _simulacaoRepo = simulacaoRepo;
            _openAi = openAi;
        }
        public async Task<RelatorioResponse> GerarRelatorioAsync()
        {
            var produtos = await _produtoRepo.GetAllAsync();
            var simulacoes = await _simulacaoRepo.GetAllAsync();

            var insights = GerarInsights(produtos, simulacoes);

            var resumo = GerarResumo(insights);

            var parecerIA = await _openAi.GerarAnaliseAsync(resumo);

            return new RelatorioResponse
            {
                ParecerIA = parecerIA,
                Insights = insights
            };
        }

        private List<InsightProduto> GerarInsights(IEnumerable<Produto> produtos, IEnumerable<Models.Simulacao.Simulacao> simulacoes)
        {
            var lista = new List<InsightProduto>();

            foreach (var produto in produtos)
            {
                var sims = simulacoes.Where(s => s.CodigoProduto == produto.CoProduto);

                if (!sims.Any())
                {
                    lista.Add(CriarInsightSemSimulacao(produto));
                    continue;
                }

                lista.Add(CriarInsightComSimulacao(produto, sims));
            }

            return lista;
        }

        private InsightProduto CriarInsightSemSimulacao(Produto produto)
        {
            return new InsightProduto
            {
                CodigoProduto = Convert.ToString(produto.CoProduto),
                NomeProduto = produto.NoProduto,
                TotalSimulacoes = 0,
                ValorMedioDesejado = 0,
                PrazoMaisFrequente = 0,
                Tendencia = "Sem simulações ainda"
            };
        }

        private InsightProduto CriarInsightComSimulacao(Produto produto, IEnumerable<Models.Simulacao.Simulacao> sims)
        {
            var prazoMaisFrequente = sims
                .GroupBy(s => s.Prazo)
                .OrderByDescending(g => g.Count())
                .First().Key;

            return new InsightProduto
            {
                CodigoProduto = Convert.ToString(produto.CoProduto),
                NomeProduto = produto.NoProduto,
                TotalSimulacoes = sims.Count(),
                ValorMedioDesejado = sims.Average(s => s.ValorDesejado),
                PrazoMaisFrequente = prazoMaisFrequente,
                Tendencia = DefinirTendencia(prazoMaisFrequente)
            };
        }

        private string DefinirTendencia(int prazoMaisFrequente)
        {
            return prazoMaisFrequente > 100
                ? "Alta procura por longo prazo"
                : "Preferência por curto/médio prazo";
        }

        private string GerarResumo(IEnumerable<InsightProduto> insights)
        {
            return string.Join("\n", insights.Select(i =>
                $"Produto {i.NomeProduto}: {i.TotalSimulacoes} simulações, valor médio {i.ValorMedioDesejado:C}, prazo mais frequente {i.PrazoMaisFrequente} meses."));
        }
    

    }
}
