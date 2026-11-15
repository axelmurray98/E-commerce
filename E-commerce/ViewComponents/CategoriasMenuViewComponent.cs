using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.ViewComponents
{
    public class CategoriasMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CategoriasMenuViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categorias = await _context.Categorias
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaMenuVm
                {
                    IdCategoria = c.IdCategoria,
                    Nombre = c.Nombre
                })
                .ToListAsync();

            return View(categorias);
        }
    }
}