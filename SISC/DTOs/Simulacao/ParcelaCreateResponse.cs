using System.ComponentModel;

namespace SISC.DTOs.Responses.Simulacao
{
    public class ParcelaCreateResponse
    {
        public int Numero { get; set; }
        public decimal ValorAmortizacao { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorPrestacao { get; set; }
    }
}
