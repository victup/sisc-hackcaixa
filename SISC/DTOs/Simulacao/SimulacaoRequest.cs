using System.ComponentModel;

namespace SISC.DTOs.Requests.Simulacao
{
    public class SimulacaoRequest
    {
        [Description("Valor que o cliente deseja simular.")]
        public decimal ValorDesejado { get; set; }

        [Description("Prazo em meses para pagamento do empréstimo.")]
        public int Prazo { get; set; }
    }
}
