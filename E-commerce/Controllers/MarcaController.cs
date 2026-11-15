using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MarcaController : Controller
    {
        private readonly AppDbContext _context;
        public MarcaController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var marcas = _context.Marcas.ToList();
            return View(marcas);
        }
        [HttpGet]
        public IActionResult Create()
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            if (!ModelState.IsValid)
            {
                
                return RedirectToAction(nameof(Index));
            }
            await _context.Marcas.AddAsync(marca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
       public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var marca = _context.Marcas.Find(id);
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Marca marca)
        {
            if (ModelState.IsValid)
            {
                _context.Marcas.Update(marca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var marca = _context.Marcas.Find(id);
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound();
            }
            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
