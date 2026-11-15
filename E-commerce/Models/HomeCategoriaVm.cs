namespace E_commerce.Models
{
    public class HomeCategoriaVm
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<Producto> Productos { get; set; } = new();
    }
}
