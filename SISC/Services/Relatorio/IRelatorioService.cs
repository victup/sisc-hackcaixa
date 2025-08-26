using SISC.DTOs.Relatorio;

namespace SISC.Services.Relatorio
{
    public interface IRelatorioService
    {
        Task<RelatorioResponse> GerarRelatorioAsync();
    }
}
