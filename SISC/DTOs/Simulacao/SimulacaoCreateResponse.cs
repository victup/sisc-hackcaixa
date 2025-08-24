using SISC.DTOs.Simulacao;

namespace SISC.DTOs.Responses.Simulacao
{
    public class SimulacaoCreateResponse
    {
        public long IdSimulacao { get; set; }
        public int CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; } = string.Empty;
        public decimal TaxaJuros { get; set; }
        public IEnumerable<ResultadoSimulacaoCreateResponse> ResultadoSimulacao { get; set; } = new List<ResultadoSimulacaoCreateResponse>();
    }
}
