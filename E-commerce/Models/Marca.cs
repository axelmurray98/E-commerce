using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Marca
    {
        public int IdMarca { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "El nombre de la marca debe tener al menos 4 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
        
        [MinLength(10, ErrorMessage = "La descripción de la marca debe tener al menos 10 caracteres.")]
        public string? Descripcion { get; set; }
        [Required]
        public bool Activo { get; set; }
        public ICollection<Producto> Productos { get; set; }
        = new List<Producto>();
    }
}
