using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<AppUsuario> _userManager;
        public PerfilController(UserManager<AppUsuario> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.GetUserAsync(User);
            if(usuarios == null)
            {
                return NotFound();
            }
            var vm = new PerfilVm
            {
                Email = usuarios.Email ?? string.Empty,
                Nombre = usuarios.Nombre,
                Apellido = usuarios.Apellido,
                FechaRegistro = usuarios.FechaRegistro
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult>Index(PerfilVm vm)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return NotFound();
            vm.Email = usuario.Email ?? string.Empty;
            vm.FechaRegistro = usuario.FechaRegistro;
            ModelState.Remove(nameof(vm.Email));
            if (!ModelState.IsValid) return View(vm);


            usuario.Nombre = vm.Nombre.Trim();
            usuario.Apellido = vm.Apellido.Trim();

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View(vm);
            }

            TempData["Ok"] = "Perfil actualizado.";
            return RedirectToAction(nameof(Index));
            
        }
    }
}
