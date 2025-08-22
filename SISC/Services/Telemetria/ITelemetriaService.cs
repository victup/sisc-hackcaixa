using SISC.DTOs.Telemetria;

namespace SISC.Services.Telemetria
{
    public interface ITelemetriaService
    {
        void Registrar(string nomeApi, long tempoMs, bool sucesso);
        TelemetriaResponse ObterEventos();
    }
}
