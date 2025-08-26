namespace SISC.DTOs.Relatorio
{
    public class RelatorioResponse
    {
        public DateTime DataReferencia { get; set; } = DateTime.UtcNow;
        public string ParecerIA { get; set; } = string.Empty;

        public List<InsightProduto> Insights { get; set; } = new();
    }
}
