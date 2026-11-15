namespace E_commerce.Models
{
    public class CarritoDetalle
    {
        public int CarritoDetalleId { get; set; }
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; } = null!;
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
