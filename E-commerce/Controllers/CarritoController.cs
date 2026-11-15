
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_commerce.Models;
using E_commerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


public class CarritoController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUsuario> _userManager;
    public CarritoController(AppDbContext context,UserManager<AppUsuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

   

    private async Task<Carrito> ObtenerCarritoAsync()
    {
        var userId = _userManager.GetUserId(User)!;

        var carrito = await _context.Carritos
            .Include(c => c.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(c => c.UsuarioId == userId);

        if (carrito == null)
        {
            carrito = new Carrito
            {
                UsuarioId = userId,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
        }

        return carrito;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var carrito = await ObtenerCarritoAsync();
        return View(carrito);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int productoId, int cantidad = 1, string? returnUrl = null)
    {
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto == null || !producto.Activo)
            return NotFound();

        var carrito = await ObtenerCarritoAsync();
        var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
        var nuevaCantidad = (detalle?.Cantidad ?? 0) + Math.Max(cantidad, 1);

        if (nuevaCantidad > producto.Stock)
        {
            TempData["Error"] = $"Solo hay {producto.Stock} unidades de {producto.Nombre}.";
        }
        else
        {
            if (detalle == null)
                _context.CarritoDetalles.Add(new CarritoDetalle
                {
                    CarritoId = carrito.CarritoId,
                    ProductoId = productoId,
                    Cantidad = nuevaCantidad
                });
            else
                detalle.Cantidad = nuevaCantidad;

            carrito.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Ok"] = $"{producto.Nombre} agregado al carrito.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Producto");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Actulizar(int detalleId,int cantidad)
    {
        var carrito = await ObtenerCarritoAsync();
        var detalle = carrito.Detalles.FirstOrDefault(d => d.CarritoDetalleId == detalleId);
        if (detalle == null) return NotFound();
        if(cantidad <= 0)
        {
            _context.CarritoDetalles.Remove(detalle);

        }else if (cantidad > detalle.Producto.Stock)
        {
            TempData["Error"] = $"Solo hay {detalle.Producto.Stock} unidades disponibles.";
        }
        else
        {
            detalle.Cantidad = cantidad;
        }
        carrito.FechaActualizacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

        
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quitar(int detalleId)
    {
        var carrito = await ObtenerCarritoAsync();
        var detalle = carrito.Detalles.FirstOrDefault(d => d.CarritoDetalleId == detalleId);
        if (detalle != null)
        {
            _context.CarritoDetalles.Remove(detalle);
            carrito.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Vaciar()
    {
        var carrito = await ObtenerCarritoAsync();
        _context.CarritoDetalles.RemoveRange(carrito.Detalles);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ComprarAhora(int productoId, int cantidad = 1)
    {
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto == null || !producto.Activo || producto.Stock < 1)
        {
            TempData["Error"] = "Este producto no está disponible.";
            return RedirectToAction("Index", "Producto");
        }

        cantidad = Math.Clamp(cantidad, 1, producto.Stock);

        var carrito = await ObtenerCarritoAsync();
        var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == productoId);

        if (detalle == null)
            _context.CarritoDetalles.Add(new CarritoDetalle
            {
                CarritoId = carrito.CarritoId,
                ProductoId = productoId,
                Cantidad = cantidad
            });
        else
            detalle.Cantidad = Math.Min(detalle.Cantidad + cantidad, producto.Stock);

        carrito.FechaActualizacion = DateTime.Now;
        await _context.SaveChangesAsync();

        return RedirectToAction("Checkout", "Pedido");
    }

}
