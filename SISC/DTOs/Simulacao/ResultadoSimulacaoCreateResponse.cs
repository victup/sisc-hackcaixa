using SISC.DTOs.Responses.Simulacao;

namespace SISC.DTOs.Simulacao
{
    public class ResultadoSimulacaoCreateResponse
    {
        public string Tipo { get; set; } = string.Empty;
        public IEnumerable<ParcelaCreateResponse> Parcelas { get; set; } = new List<ParcelaCreateResponse>();
    }
}
