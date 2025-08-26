namespace SISC.DTOs.Relatorio
{
    public class InsightProduto
    {
        public string CodigoProduto { get; set; } = string.Empty;
        public string NomeProduto { get; set; } = string.Empty;
        public int TotalSimulacoes { get; set; }
        public decimal ValorMedioDesejado { get; set; }
        public int PrazoMaisFrequente { get; set; }
        public string Tendencia { get; set; } = string.Empty;
    }
}
