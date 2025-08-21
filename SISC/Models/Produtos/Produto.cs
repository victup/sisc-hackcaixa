    namespace SISC.Models.Produto
{
    using System.ComponentModel;

    public class Produto
    {
        [Description("Código único do produto.")]
        public int CoProduto { get; set; }

        [Description("Nome do produto")]
        public string NoProduto { get; set; }

        [Description("Taxa de juros mensal aplicada ao produto")]
        public decimal PcTaxaJuros { get; set; }

        [Description("Número mínimo de meses para pagamento do empréstimo")]
        public short NuMinimoMeses { get; set; }

        [Description("Número máximo de meses para pagamento do empréstimo")]
        public short NuMaximoMeses { get; set; }

        [Description("Valor mínimo que pode ser solicitado no empréstimo")]
        public decimal VrMinimo { get; set; }

        [Description("Valor máximo que pode ser solicitado no empréstimo")]
        public decimal VrMaximo { get; set; }
    }

}
