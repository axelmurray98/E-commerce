using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models
{
    public enum MetodoPago
    {
        Efectivo,
        [Display(Name = "Tarjeta de crédito")] TarjetaCredito,
        [Display(Name = "Tarjeta de débito")] TarjetaDebito,
        Transferencia,
        [Display(Name = "Mercado Pago")] MercadoPago
    }
}
