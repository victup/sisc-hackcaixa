using System.ComponentModel;

namespace SISC.Models.Simulacao
{
    public class Parcela
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public decimal ValorAmortizacao { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorPrestacao { get; set; }

        public int ResultadoSimulacaoId { get; set; }
        public ResultadoSimulacao Resultado { get; set; } = null!;
    }
}
