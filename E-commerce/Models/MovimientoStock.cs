using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class MovimientoStock
    {
        public int MovimientoStockId { get; set; }
        [Required]
        public int ProductoId { get; set; }
        [Required]
        public TipoMovimientoStock TipoMovimiento { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        [MinLength(10, ErrorMessage = "El motivo del movimiento debe tener al menos 10 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
        public string? UsuarioId { get; set; }
    }
}
