using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace E_commerce.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            const int porCategoria = 10;
            const int maxTotal = 15;

            var categoria = await _context.Categorias.Where(c => c.Activo)
                .OrderBy(c => c.Activo)
                .ToListAsync();
            var resultado = new List<HomeCategoriaVm>();
            var restantes = maxTotal;
            foreach (var cat in categoria)
            {
                if (restantes <= 0) break;
                var productos = await _context.Productos
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .Where(p => p.Activo && p.Stock > 0 && p.CategoriaId == cat.IdCategoria)
                    .Take(Math.Min(porCategoria, restantes)).ToListAsync();

                if (!productos.Any()) continue;

                restantes -= productos.Count;

                resultado.Add(new HomeCategoriaVm
                {
                    IdCategoria = cat.IdCategoria,
                    Nombre = cat.Nombre,
                    Productos = productos
                });
            }
            return View(resultado);
        }
        
        
       
    }
}
