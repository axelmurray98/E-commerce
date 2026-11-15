using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        public AppUsuario Usuario { get; set; } = null!;
        [Required]
        [Display(Name = "Fecha del pedido")]
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que cero.")]
        public decimal Total { get; set; }
        [Required]
        public EstadoPedido Estado { get; set; } 
        [Required]
        [Display(Name = "Método de pago")]
        public MetodoPago MetodoPago { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "La calle debe tener al menos 3 caracteres.")]
        public string Calle { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Número")]
        [MinLength(1, ErrorMessage = "El número debe tener al menos 1 carácter.")]
        public string Numero { get; set; } = string.Empty;
        [Required]
        [MinLength(2, ErrorMessage = "La ciudad debe tener al menos 2 caracteres.")]
        public string Ciudad { get; set; } = string.Empty;
        [Required]
        [MinLength(2, ErrorMessage = "La provincia debe tener al menos 2 caracteres.")]
        public string Provincia { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Código Postal")]
        [MinLength(4, ErrorMessage = "El código postal debe tener al menos 4 caracteres.")]
        [MaxLength(8, ErrorMessage = "El código postal no puede superar los 8 caracteres.")]
        public string CodigoPostal { get; set; } = string.Empty;

    }
}
