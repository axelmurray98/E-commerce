using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUsuario> _useManager;
        public PedidoController(AppDbContext context, UserManager<AppUsuario> useManager)
        {
            _context = context;
            _useManager = useManager;
        }

        public async Task<IActionResult> MisPedidos()
        {
            var userId = _useManager.GetUserId(User);
            var pedidos = await _context.Pedidos
            .Where(p => p.UsuarioId == userId)
            .OrderByDescending(p => p.PedidoId)
            .ToListAsync();
            return View(pedidos);
        }
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var userId =  _useManager.GetUserId(User);
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.PedidoId == id);
            if (pedido == null) return NotFound();
            ViewBag.Detalles = await _context.PedidoDetalles
                .Where(d => d.PedidoId == id)
                .ToListAsync();
            
            return View(pedido);

        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = _useManager.GetUserId(User);
            var tieneItems = await _context.CarritoDetalles
                .AnyAsync(d => d.Carrito.UsuarioId == userId);
            if (!tieneItems)
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }
            return View(new Pedido());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Pedido modelo)
        {
            // Campos que el usuario no envía
            ModelState.Remove(nameof(Pedido.UsuarioId));
            ModelState.Remove(nameof(Pedido.Usuario));
            ModelState.Remove(nameof(Pedido.FechaPedido));
            ModelState.Remove(nameof(Pedido.Total));
            ModelState.Remove(nameof(Pedido.Estado));

            if (!ModelState.IsValid)
                return View(modelo);

            var userId = _useManager.GetUserId(User)!;

            var carrito = await _context.Carritos
                .Include(c => c.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.Detalles.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            // Validación previa de stock
            foreach (var d in carrito.Detalles)
            {
                if (!d.Producto.Activo || d.Cantidad > d.Producto.Stock)
                {
                    TempData["Error"] = $"No hay stock suficiente de {d.Producto.Nombre}.";
                    return RedirectToAction("Index", "Carrito");
                }
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            var pagoInmediato = modelo.MetodoPago == MetodoPago.TarjetaCredito
                             || modelo.MetodoPago == MetodoPago.TarjetaDebito;

            var pedido = new Pedido
            {
                UsuarioId = userId,
                FechaPedido = DateTime.Now,
                MetodoPago = modelo.MetodoPago,
                Estado = pagoInmediato ? EstadoPedido.Pagado : EstadoPedido.Pendiente,
                Calle = modelo.Calle,
                Numero = modelo.Numero,
                Ciudad = modelo.Ciudad,
                Provincia = modelo.Provincia,
                CodigoPostal = modelo.CodigoPostal,
                Total = carrito.Detalles.Sum(d => d.Cantidad * d.Producto.Precio)
            };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync(); // genera PedidoId

            foreach (var d in carrito.Detalles)
            {
                // Revalidar dentro de la transacción
                if (d.Producto.Stock < d.Cantidad)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = $"No hay stock suficiente de {d.Producto.Nombre}.";
                    return RedirectToAction("Index", "Carrito");
                }

                _context.PedidoDetalles.Add(new PedidoDetalle
                {
                    PedidoId = pedido.PedidoId,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    PrecioUnitario = d.Producto.Precio,
                    Cantidad = d.Cantidad,
                    Subtotal = d.Cantidad * d.Producto.Precio
                });

                d.Producto.Stock -= d.Cantidad;

                _context.MovimientosStock.Add(new MovimientoStock
                {
                    ProductoId = d.ProductoId,
                    TipoMovimiento = TipoMovimientoStock.Salida,
                    Cantidad = d.Cantidad,                      // siempre positiva
                    Fecha = DateTime.Now,
                    Motivo = $"Venta - Pedido #{pedido.PedidoId}",
                    UsuarioId = userId
                });
            }

            _context.CarritoDetalles.RemoveRange(carrito.Detalles);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Ok"] = "¡Compra realizada!";
            return RedirectToAction(nameof(Detalle), new { id = pedido.PedidoId });
        }
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Admin(int pagina = 1)
        {
            const int porPagina = 10;

            var total = await _context.Pedidos.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            pagina = Math.Clamp(pagina, 1, Math.Max(totalPaginas, 1));

            var pedidos = await _context.Pedidos
                .OrderByDescending(p => p.PedidoId)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(pedidos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, EstadoPedido estado, int pagina = 1)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            if (pedido.Estado == estado)
                return RedirectToAction(nameof(Admin), new { pagina });

            using var tx = await _context.Database.BeginTransactionAsync();

            // Al cancelar, devolver el stock (solo si antes no estaba cancelado)
            if (estado == EstadoPedido.Cancelado && pedido.Estado != EstadoPedido.Cancelado)
            {
                var detalles = await _context.PedidoDetalles
                    .Where(d => d.PedidoId == id)
                    .ToListAsync();

                foreach (var d in detalles)
                {
                    var producto = await _context.Productos.FindAsync(d.ProductoId);
                    if (producto == null) continue;

                    producto.Stock += d.Cantidad;

                    _context.MovimientosStock.Add(new MovimientoStock
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        Motivo = $"Cancelación - Pedido #{id}",
                        Fecha = DateTime.Now
                    });
                }
            }

            pedido.Estado = estado;
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Ok"] = $"Pedido #{id} actualizado a {estado}.";
            return RedirectToAction(nameof(Admin));
        }



    }
}
