using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;

namespace SISC.Services.Simulacao
{
    public interface ISimulacaoService
    {
        Task<SimulacaoResponse> CriarSimulacaoAsync(SimulacaoRequest request);
        Task<IEnumerable<SimulacaoResponse>> ObterTodasAsync();
        Task<IEnumerable<SimulacaoResponse>> ObterPorDataAsync(DateTime data);
    }
}
