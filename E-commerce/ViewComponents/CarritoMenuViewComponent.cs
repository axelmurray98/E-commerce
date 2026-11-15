using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.ViewComponents
{
    public class CarritoMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUsuario> _userManager;

        public CarritoMenuViewComponent(AppDbContext context, UserManager<AppUsuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var detalles = new List<CarritoDetalle>();

            if (UserClaimsPrincipal.Identity?.IsAuthenticated ?? false)
            {
                var userId = _userManager.GetUserId(UserClaimsPrincipal);
                detalles = await _context.CarritoDetalles
                    .Include(d => d.Producto)
                    .Where(d => d.Carrito.UsuarioId == userId)
                    .ToListAsync();
            }

            return View(detalles);
        }
    }
}