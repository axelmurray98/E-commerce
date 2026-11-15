using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Models
{
    public class AppUsuario : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public Carrito? Carrito { get; set; } 
    }
}
