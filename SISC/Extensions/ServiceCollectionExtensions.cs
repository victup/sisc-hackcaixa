using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;
using SISC.Services.Produtos;
using SISC.Services.Simulacao;
using SISC.Services.Telemetria;

namespace SISC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSiscServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();

            // Services
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<ISimulacaoService, SimulacaoService>();

            services.AddSingleton<ITelemetriaService, TelemetriaService>();

            return services;
        }
    }
}
