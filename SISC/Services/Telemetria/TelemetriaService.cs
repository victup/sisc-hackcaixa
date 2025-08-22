using SISC.DTOs.Telemetria;
using SISC.Services.Telemetria;
using System.Collections.Concurrent;

public class TelemetriaService : ITelemetriaService
{
    private class Stats
    {
        public int TotalRequisicoes { get; set; }
        public long TempoTotal { get; set; }
        public long TempoMinimo { get; set; } = long.MaxValue;
        public long TempoMaximo { get; set; } = long.MinValue;
        public int Sucessos { get; set; }
    }

    private static readonly ConcurrentDictionary<string, Stats> _estatisticas = new();

    public void Registrar(string nomeApi, long tempoMs, bool sucesso)
    {
        var stats = _estatisticas.GetOrAdd(nomeApi, _ => new Stats());

        lock (stats)
        {
            stats.TotalRequisicoes++;
            stats.TempoTotal += tempoMs;
            stats.TempoMinimo = Math.Min(stats.TempoMinimo, tempoMs);
            stats.TempoMaximo = Math.Max(stats.TempoMaximo, tempoMs);
            if (sucesso) stats.Sucessos++;
        }
    }

    public TelemetriaResponse ObterEventos()
    {
        return new TelemetriaResponse
        {
            DataReferencia = DateTime.UtcNow.Date,
            ListaEndpoints = _estatisticas.Select(kvp =>
            {
                var s = kvp.Value;
                return new EndpointTelemetriaResponse
                {
                    NomeApi = kvp.Key,
                    QtdRequisicoes = s.TotalRequisicoes,
                    TempoMedio = s.TotalRequisicoes > 0 ? s.TempoTotal / s.TotalRequisicoes : 0,
                    TempoMinimo = s.TempoMinimo == long.MaxValue ? 0 : s.TempoMinimo,
                    TempoMaximo = s.TempoMaximo == long.MinValue ? 0 : s.TempoMaximo,
                    PercentualSucesso = s.TotalRequisicoes > 0 ? (double)s.Sucessos / s.TotalRequisicoes : 0
                };
            }).ToList()
        };
    }
}