using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_commerce.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductoController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index(int? categoriaId, string? buscar)
        {
            var query = _context.Productos
                .Where(p => p.Activo && p.Stock >= 1)
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .AsQueryable();

            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaId == categoriaId.Value);

                ViewBag.CategoriaNombre = await _context.Categorias
                    .Where(c => c.IdCategoria == categoriaId.Value)
                    .Select(c => c.Nombre)
                    .FirstOrDefaultAsync();
            }

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar));
                ViewBag.Buscar = buscar;
            }

            var productos = await query.ToListAsync();
            return View(productos);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Marcas = new SelectList(_context.Marcas.Where(m => m.Activo), "IdMarca", "Nombre");
            ViewBag.Categorias = new SelectList(_context.Categorias.Where(c => c.Activo), "IdCategoria", "Nombre");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto, IFormFile imagen)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Marcas = new SelectList(_context.Marcas.Where(m => m.Activo), "IdMarca", "Nombre");
                ViewBag.Categorias = new SelectList(_context.Categorias.Where(c => c.Activo), "IdCategoria", "Nombre");
                return View(producto);
            }

            // Validar código duplicado
            if (await _context.Productos.AnyAsync(p => p.SKU == producto.SKU))
            {
                ModelState.AddModelError("SKU", "Ya existe un producto con ese código.");

                ViewBag.Marcas = new SelectList(_context.Marcas.Where(m => m.Activo), "IdMarca", "Nombre");
                ViewBag.Categorias = new SelectList(_context.Categorias.Where(c => c.Activo), "IdCategoria", "Nombre");

                return View(producto);
            }

            if (imagen != null && imagen.Length > 0)
            {
                var nombreArchivo =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(imagen.FileName);

                var rutaCarpeta = Path.Combine(
                    _environment.WebRootPath,
                    "Img-Productos");

                var rutaCompleta = Path.Combine(
                    rutaCarpeta,
                    nombreArchivo);

                using (var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                producto.ImagenUrl =
                    "/Img-Productos/" + nombreArchivo;
            }

            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            var producto = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .FirstOrDefault(p => p.IdProducto == id);
            if(producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var producto = await _context.Productos
        .FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewBag.Marcas = new SelectList(_context.Marcas.Where(m => m.Activo), "IdMarca", "Nombre");
            ViewBag.Categorias = new SelectList(_context.Categorias.Where(c => c.Activo), "IdCategoria", "Nombre");
            return View(producto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    Producto producto,
    IFormFile? imagen,
    bool EliminarImagen)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Marcas = new SelectList(
                    _context.Marcas.Where(m => m.Activo),
                    "IdMarca",
                    "Nombre");

                ViewBag.Categorias = new SelectList(
                    _context.Categorias.Where(c => c.Activo),
                    "IdCategoria",
                    "Nombre");

                return View(producto);
            }

            var productoDb = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (productoDb == null)
            {
                return NotFound();
            }

            // Validar SKU duplicado
            if (await _context.Productos.AnyAsync(p =>
                p.SKU == producto.SKU &&
                p.IdProducto != producto.IdProducto))
            {
                ModelState.AddModelError(nameof(Producto.SKU),
                    "Ya existe un producto con ese código.");

                ViewBag.Marcas = new SelectList(
                    _context.Marcas.Where(m => m.Activo),
                    "IdMarca",
                    "Nombre");

                ViewBag.Categorias = new SelectList(
                    _context.Categorias.Where(c => c.Activo),
                    "IdCategoria",
                    "Nombre");

                return View(producto);
            }

            productoDb.Nombre = producto.Nombre;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.SKU = producto.SKU;
            productoDb.Costo = producto.Costo;
            productoDb.Precio = producto.Precio;
            productoDb.Stock = producto.Stock;
            productoDb.Activo = producto.Activo;
            productoDb.MarcaId = producto.MarcaId;
            productoDb.CategoriaId = producto.CategoriaId;

            // resto de tu código de imágenes...

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PanelproductoAdmin(
    string? buscar, int? marcaId, int? categoriaId, int pagina = 1)
        {
            const int porPagina = 4;

            // ===== Consulta principal =====
            var query = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                query = query.Where(p => p.Nombre.Contains(buscar) || p.SKU.Contains(buscar));

            if (marcaId.HasValue)
                query = query.Where(p => p.MarcaId == marcaId.Value);

            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId.Value);

            // ===== Paginación =====
            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            pagina = Math.Clamp(pagina, 1, Math.Max(totalPaginas, 1));

            var productos = await query
                .OrderByDescending(p => p.IdProducto)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            // ===== Marcas: solo las que tienen productos en la categoría elegida =====
            var marcasQuery = _context.Productos.AsQueryable();
            if (categoriaId.HasValue)
                marcasQuery = marcasQuery.Where(p => p.CategoriaId == categoriaId.Value);

            var marcas = await marcasQuery
                .Where(p => p.Marca != null)
                .Select(p => p.Marca!)
                .Distinct()
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            // ===== Categorías: solo las que tienen productos de la marca elegida =====
            var categoriasQuery = _context.Productos.AsQueryable();
            if (marcaId.HasValue)
                categoriasQuery = categoriasQuery.Where(p => p.MarcaId == marcaId.Value);

            var categorias = await categoriasQuery
                .Where(p => p.Categoria != null)
                .Select(p => p.Categoria!)
                .Distinct()
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            // ===== ViewBag para la vista =====
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Buscar = buscar;
            ViewBag.MarcaId = marcaId;
            ViewBag.CategoriaId = categoriaId;

            ViewBag.Marcas = new SelectList(marcas, "IdMarca", "Nombre", marcaId);
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", categoriaId);
            ViewBag.ListaMarcas = marcas;
            ViewBag.ListaCategorias = categorias;

            return View(productos);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var producto = await _context.Productos.
                FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null) return NotFound();
            if (!string.IsNullOrEmpty(producto.ImagenUrl))
            {
                 var rutaImagen = Path.Combine(
            _environment.WebRootPath,
            producto.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);
                }
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PanelproductoAdmin));
        }
       

    }
}
