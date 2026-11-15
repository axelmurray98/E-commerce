namespace E_commerce.Models
{
    public class PedidoDetalle
    {
        public int PedidoDetalleId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}
