using SISC.DTOs.Responses.Simulacao;

namespace SISC.DTOs.Simulacao
{
    public class ResultadoSimulacaoResponse
    {
        public string Tipo { get; set; } = string.Empty;
        public IEnumerable<ParcelaResponse> Parcelas { get; set; } = new List<ParcelaResponse>();
    }
}
