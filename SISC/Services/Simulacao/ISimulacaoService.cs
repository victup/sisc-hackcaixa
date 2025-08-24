using SISC.DTOs.Requests.Simulacao;
using SISC.DTOs.Responses.Simulacao;
using SISC.DTOs.Simulacao;

namespace SISC.Services.Simulacao
{
    public interface ISimulacaoService
    {
        Task<SimulacaoCreateResponse> CriarSimulacaoAsync(SimulacaoRequest request);
        Task<SimulacaoGetAllResponse> ObterTodasAsync(int pagina = 1, int qtdPorPagina = 10);
        Task<SimulacaoByDiaResponse> ObterPorDataAsync(DateTime data);
    }
}
