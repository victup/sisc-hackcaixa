namespace SISC.DTOs.Simulacao
{
    public class ResultadoSimulacaoByDiaResponse
    {
        public long codigoProduto { get; set; }
        public string descricaoProduto { get; set; } = string.Empty;
        public decimal taxaMediaJuro { get; set; }
        public decimal valorMedioPrestacao { get; set; }
        public decimal valorTotalDesejado { get; set; }
        public decimal valorTotalCredito { get; set; }
    }
}
