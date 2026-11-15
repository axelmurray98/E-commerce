using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "El nombre del producto debe tener al menos 4 caracteres.")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; } = string.Empty;
        
        [MinLength(20, ErrorMessage = "La descripción del producto debe tener al menos 10 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        [Required]
        [Unicode]
        [Display(Name = "Código del producto")]
        public string? SKU { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor que cero.")]
        public decimal Costo { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]

        public int Stock { get; set; }
        [MinLength(4, ErrorMessage = "La URL de la imagen debe tener al menos 4 caracteres.")]
        public string? ImagenUrl { get; set; }
        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }
        [Display(Name = "Marca")]
        public int? MarcaId { get; set; }
        [Required]
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Categoria? Categoria { get; set; }
        public Marca? Marca { get; set; }

        public ICollection<CarritoDetalle> Carritos { get; set; }
            = new List<CarritoDetalle>();

        public ICollection<PedidoDetalle> Pedidos { get; set; }
            = new List<PedidoDetalle>();

        public ICollection<MovimientoStock> MovimientosStock { get; set; }
            = new List<MovimientoStock>();

    }
}
