using System.ComponentModel;

namespace SISC.DTOs.Responses.Simulacao
{
    public class ParcelaResponse
    {
        [Description("Número da parcela.")]
        public int Numero { get; set; }

        [Description("Valor da amortização.")]
        public decimal ValorAmortizacao { get; set; }

        [Description("Valor dos juros.")]
        public decimal ValorJuros { get; set; }

        [Description("Valor total da prestação.")]
        public decimal ValorPrestacao { get; set; }
    }
}
