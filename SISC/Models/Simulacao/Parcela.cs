using System.ComponentModel;

namespace SISC.Models.Simulacao
{
    public class Parcela
    {
        [Description("Identificador único da Parcela.")]
        public int IdParcela { get; set; }

        [Description("Número sequencial da parcela.")]
        public int Numero { get; set; }

        [Description("Valor da amortização na parcela.")]
        public decimal ValorAmortizacao { get; set; }

        [Description("Valor dos juros na parcela.")]
        public decimal ValorJuros { get; set; }

        [Description("Valor total da parcela (amortização + juros).")]
        public decimal ValorPrestacao { get; set; }

        [Description("Chave estrangeira para a simulação.")]
        public long IdSimulacao { get; set; }

        [Description("Simulação associada a esta parcela.")]
        public Simulacao Simulacao { get; set; } = null!;
    }
}
