using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public AppUsuario Usuario { get; set; } = null!;

        [Required]
        public DateTime FechaCreacion { get; set; }
        [Required]
        public DateTime FechaActualizacion { get; set; }

        public ICollection<CarritoDetalle> Detalles { get; set; } = new List<CarritoDetalle>();
    }
}
