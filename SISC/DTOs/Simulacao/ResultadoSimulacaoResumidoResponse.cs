namespace SISC.DTOs.Simulacao
{
    public class ResultadoSimulacaoResumidoResponse
    {
        public long IdSimulacao { get; set; }
        public int CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; } = string.Empty;
        public decimal valorDesejado { get; set; }
        public int Prazo { get; set; }
        public decimal valorTotalParcelas { get; set; }

    }
}
