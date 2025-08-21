using System.ComponentModel;

namespace SISC.Models.Simulacao
{
    public class Simulacao
    {
        [Description("Identificador único da simulação.")]
        public long IdSimulacao { get; set; }

        [Description("Código do produto associado à simulação.")]
        public int CodigoProduto { get; set; }

        [Description("Descrição do produto associado à simulação.")]
        public string DescricaoProduto { get; set; } = string.Empty;

        [Description("Valor que o cliente deseja simular.")]
        public decimal ValorDesejado { get; set; }

        [Description("Quantidade de parcelas (prazo em meses).")]
        public int Prazo { get; set; }

        [Description("Taxa de juros aplicada na simulação.")]
        public decimal TaxaJuros { get; set; }

        [Description("Tipo de amortização utilizada (SAC ou PRICE).")]
        public string Tipo { get; set; } = string.Empty;

        [Description("Lista de parcelas calculadas na simulação.")]
        public List<Parcela> Parcelas { get; set; } = new();
    }
}
