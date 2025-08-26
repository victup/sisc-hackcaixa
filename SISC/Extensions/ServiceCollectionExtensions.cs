using SISC.Integrations;
using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;
using SISC.Services.Produtos;
using SISC.Services.Relatorio;
using SISC.Services.Simulacao;
using SISC.Services.Telemetria;

namespace SISC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSiscServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Repositories
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();

            // Services
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<ISimulacaoService, SimulacaoService>();
            services.AddScoped<IRelatorioService, RelatorioService>();

            services.AddSingleton<ITelemetriaService, TelemetriaService>();

            services.AddScoped<IOpenAiClient>(_ =>
            new OpenAiClient(configuration["OpenAI:ApiKey"]));

            return services;
        }
    }
}
