using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace E_commerce.Models
{
    public class Direccion
    {
        public int DireccionId { get; set; }
        [Required]
        public string UsuarioId { get; set; }  = string.Empty;
        public AppUsuario Usuario { get; set; } = null!;
        [Required]
        [MinLength(3, ErrorMessage = "La calle debe tener al menos 3 caracteres.")]
        public string Calle { get; set; } = string.Empty;
        [Required]
        
        [Display(Name = "Número")]
        public string Numero { get; set; } = string.Empty;
        [Required]
        [MinLength(3, ErrorMessage = "La ciudad debe tener al menos 3 caracteres.")]
        
        public string Ciudad { get; set; } = string.Empty;
        [Required]
        [MinLength(3, ErrorMessage = "La provincia debe tener al menos 3 caracteres.")]

        public string Provincia { get; set; } = string.Empty;
        [Required]
        [MinLength(3, ErrorMessage = "El código postal debe tener al menos 3 caracteres.")]
        [Display(Name = "Código Postal")]
        public string CodigoPostal { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
