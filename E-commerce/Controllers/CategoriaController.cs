using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;
        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categorias = _context.Categorias.ToList();
            return View(categorias);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Add(categoria);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            return View(categoria);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Categoria categoria)
        {
            if (id != categoria.IdCategoria)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var categoriaDb = _context.Categorias.Find(id);

            if (categoriaDb == null)
            {
                return NotFound();
            }

            if (_context.Categorias.Any(c =>
                c.Nombre == categoria.Nombre &&
                c.IdCategoria != categoria.IdCategoria))
            {
                ModelState.AddModelError(nameof(Categoria.Nombre),
                    "Ya existe una categoría con ese nombre.");

                return View(categoria);
            }

            categoriaDb.Nombre = categoria.Nombre;
            categoriaDb.Descripcion = categoria.Descripcion;
            categoriaDb.Activo = categoria.Activo;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        
        public IActionResult Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
