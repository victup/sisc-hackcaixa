using SISC.DTOs.Relatorio;
using SISC.Integrations;
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

            var insights = produtos.Select(p =>
            {
                var sims = simulacoes.Where(s => s.CodigoProduto == p.CoProduto);

                if (!sims.Any())
                {
                    return new InsightProduto
                    {
                        CodigoProduto = Convert.ToString(p.CoProduto),
                        NomeProduto = p.NoProduto,
                        TotalSimulacoes = 0,
                        ValorMedioDesejado = 0,
                        PrazoMaisFrequente = 0,
                        Tendencia = "Sem simulações ainda"
                    };
                }

                var prazoMaisFrequente = sims
                    .GroupBy(s => s.Prazo)
                    .OrderByDescending(g => g.Count())
                    .First().Key;

                return new InsightProduto
                {
                    CodigoProduto = Convert.ToString(p.CoProduto),
                    NomeProduto = p.NoProduto,
                    TotalSimulacoes = sims.Count(),
                    ValorMedioDesejado = sims.Average(s => s.ValorDesejado),
                    PrazoMaisFrequente = prazoMaisFrequente,
                    Tendencia = prazoMaisFrequente > 100 ? "Alta procura por longo prazo" : "Preferência por curto/médio prazo"
                };
            }).ToList();

            var resumo = string.Join("\n", insights.Select(i =>
                $"Produto {i.NomeProduto}: {i.TotalSimulacoes} simulações, valor médio {i.ValorMedioDesejado:C}, prazo mais frequente {i.PrazoMaisFrequente} meses."));

            var parecerIA = await _openAi.GerarAnaliseAsync(resumo);

            return new RelatorioResponse
            {
                ParecerIA = parecerIA,
                Insights = insights
            };
        }
    }
}
