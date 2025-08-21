using System.ComponentModel;

namespace SISC.DTOs.Responses.Simulacao
{
    public class SimulacaoResponse
    {
        [Description("Identificador único da simulação.")]
        public long IdSimulacao { get; set; }

        [Description("Código do produto associado.")]
        public int CodigoProduto { get; set; }

        [Description("Descrição do produto associado.")]
        public string DescricaoProduto { get; set; }

        [Description("Valor solicitado na simulação.")]
        public decimal ValorDesejado { get; set; }

        [Description("Quantidade de meses para pagamento.")]
        public int Prazo { get; set; }

        [Description("Taxa de juros aplicada.")]
        public decimal TaxaJuros { get; set; }

        [Description("Tipo de amortização utilizada (SAC ou PRICE).")]
        public string Tipo { get; set; }

        [Description("Lista de parcelas calculadas.")]
        public List<ParcelaResponse> Parcelas { get; set; } = new();
    }
}
