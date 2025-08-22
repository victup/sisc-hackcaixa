using SISC.Repositories.Produtos;
using SISC.Repositories.Simulacao;
using SISC.Services.Produtos;
using SISC.Services.Simulacao;

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

            return services;
        }
    }
}
