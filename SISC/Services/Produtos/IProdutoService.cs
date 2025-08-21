using SISC.Models.Produto;

namespace SISC.Services.Produtos
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> ListarProdutosAsync();
        Task<Produto?> BuscarProdutoAsync(int id);
    }
}
