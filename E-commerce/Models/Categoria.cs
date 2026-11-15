using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "El nombre de la categoría debe tener al menos 4 caracteres.")]
        [Display(Name = "Nombre de la categoría")]
        public string Nombre { get; set; } = string.Empty;

        [MinLength(5, ErrorMessage = "La descripción de la categoría debe tener al menos 5 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
       
        public bool Activo { get; set; }
        public ICollection<Producto> Productos { get; set; }
        = new List<Producto>();

    }
}
