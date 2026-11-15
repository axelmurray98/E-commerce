using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class PerfilVm
    {
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
    }
}
