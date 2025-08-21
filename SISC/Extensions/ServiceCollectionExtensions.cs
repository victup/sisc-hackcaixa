using SISC.Repositories.Produtos;
using SISC.Services.Produtos;

namespace SISC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSiscServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            // Services
            services.AddScoped<IProdutoService, ProdutoService>();

            return services;
        }
    }
}
