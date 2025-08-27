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
            AtualizarEstatisticas(stats, tempoMs, sucesso);
        }
    }

    public TelemetriaResponse ObterEventos()
    {
        return new TelemetriaResponse
        {
            DataReferencia = DateTime.UtcNow.Date,
            ListaEndpoints = MapearEndpoints()
        };
    }

    private void AtualizarEstatisticas(Stats stats, long tempoMs, bool sucesso)
    {
        stats.TotalRequisicoes++;
        stats.TempoTotal += tempoMs;
        stats.TempoMinimo = Math.Min(stats.TempoMinimo, tempoMs);
        stats.TempoMaximo = Math.Max(stats.TempoMaximo, tempoMs);
        if (sucesso)
        {
            stats.Sucessos++;
        }
    }

    private List<EndpointTelemetriaResponse> MapearEndpoints()
    {
        return _estatisticas
            .Select(kvp => CriarEndpointResponse(kvp.Key, kvp.Value))
            .ToList();
    }

    private EndpointTelemetriaResponse CriarEndpointResponse(string nomeApi, Stats stats)
    {
        return new EndpointTelemetriaResponse
        {
            NomeApi = nomeApi,
            QtdRequisicoes = stats.TotalRequisicoes,
            TempoMedio = CalcularTempoMedio(stats),
            TempoMinimo = stats.TempoMinimo == long.MaxValue ? 0 : stats.TempoMinimo,
            TempoMaximo = stats.TempoMaximo == long.MinValue ? 0 : stats.TempoMaximo,
            PercentualSucesso = CalcularPercentualSucesso(stats)
        };
    }

    private long CalcularTempoMedio(Stats stats)
    {
        return stats.TotalRequisicoes > 0
            ? stats.TempoTotal / stats.TotalRequisicoes
            : 0;
    }

    private double CalcularPercentualSucesso(Stats stats)
    {
        return stats.TotalRequisicoes > 0
            ? (double)stats.Sucessos / stats.TotalRequisicoes
            : 0;
    }
}