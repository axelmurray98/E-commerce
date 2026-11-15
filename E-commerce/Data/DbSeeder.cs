using E_commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUsuario>>();

            // Aplica migraciones pendientes
            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);
            await SeedCategoriasAsync(context);
            await SeedMarcasAsync(context);
            await SeedProductosAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var rol in new[] { "Admin", "Cliente" })
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        private static async Task SeedAdminAsync(UserManager<AppUsuario> userManager)
        {
            const string email = "Admin@Admin.com";
            const string password = "123456"; // cambiala después de entrar

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new AppUsuario
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Nombre = "Admin",
                    Apellido = "admin",
                    FechaRegistro = DateTime.Now
                };

                var result = await userManager.CreateAsync(admin, password);
                if (!result.Succeeded)
                    throw new Exception("No se pudo crear el admin: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        private static async Task SeedCategoriasAsync(AppDbContext context)
        {
            var categorias = new[]
            {
        new Categoria
        {
            Nombre = "Remeras",
            Descripcion = "Prendas superiores",
            Activo = true
        },
        new Categoria
        {
            Nombre = "Pantalones",
            Descripcion = "Prendas inferiores",
            Activo = true
        },
        new Categoria
        {
            Nombre = "Camperas",
            Descripcion = "Abrigos y camperas",
            Activo = true
        },
        new Categoria
        {
            Nombre = "Calzado",
            Descripcion = "Zapatillas y calzado",
            Activo = true
        },
        new Categoria
        {
            Nombre = "Accesorios",
            Descripcion = "Complementos varios",
            Activo = true
        }
    };

            foreach (var categoria in categorias)
            {
                var categoriaDb = await context.Categorias
                    .FirstOrDefaultAsync(c => c.Nombre == categoria.Nombre);

                if (categoriaDb == null)
                {
                    context.Categorias.Add(categoria);
                }
                else
                {
                    categoriaDb.Descripcion = categoria.Descripcion;
                    categoriaDb.Activo = categoria.Activo;
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedMarcasAsync(AppDbContext context)
        {
            var marcas = new[]
            {
        new Marca
        {
            Nombre = "Nike",
            Descripcion = "Marca deportiva internacional",
            Activo = true
        },
        new Marca
        {
            Nombre = "Adidas",
            Descripcion = "Indumentaria y calzado deportivo",
             Activo = true
        },
        new Marca
        {
            Nombre = "Puma",
            Descripcion = "Moda urbana y deportiva",
             Activo = true
        },
        new Marca
        {
            Nombre = "Levi's",
            Descripcion = "Especialista en jeans y ropa casual",
             Activo = true
        }
    };

            foreach (var marca in marcas)
            {
                var marcaDb = await context.Marcas
                    .FirstOrDefaultAsync(m => m.Nombre == marca.Nombre);

                if (marcaDb == null)
                {
                    context.Marcas.Add(marca);
                }
                else
                {
                    marcaDb.Descripcion = marca.Descripcion;
                }
            }

            await context.SaveChangesAsync();
        }
        private static async Task SeedProductosAsync(AppDbContext context)
        {
            var remeras = await context.Categorias
        .FirstAsync(c => c.Nombre == "Remeras");

            var pantalones = await context.Categorias
                .FirstAsync(c => c.Nombre == "Pantalones");

            var calzado = await context.Categorias
                .FirstAsync(c => c.Nombre == "Calzado");

            var nike = await context.Marcas
                .FirstAsync(m => m.Nombre == "Nike");

            var adidas = await context.Marcas
                .FirstAsync(m => m.Nombre == "Adidas");

            var levis = await context.Marcas
                .FirstAsync(m => m.Nombre == "Levi's");
            var productos = new List<Producto>
    {
        new Producto
        {
            Nombre = "Remera Dry-Fit",
            Descripcion = "Remera deportiva de secado rápido",
            SKU = "REM-NIKE-001",
            Precio = 25000m,
            Costo = 15000m,
            Stock = 30,
            CategoriaId = remeras.IdCategoria,
            MarcaId = nike.IdMarca,
            ImagenUrl = "/img-productos/no-img.png",
            Activo = true,
            FechaCreacion = DateTime.Now
        },
        new Producto
        {
            Nombre = "Remera Trefoil",
            Descripcion = "Remera de algodón con logo clásico",
            SKU = "REM-ADI-001",
            Precio = 22000m,
            Costo = 13000m,
            Stock = 25,
            CategoriaId = remeras.IdCategoria,
            MarcaId = adidas.IdMarca,
            ImagenUrl = "/img-productos/no-img.png",
            Activo = true,
            FechaCreacion = DateTime.Now
        },
        new Producto
        {
            Nombre = "Jean 501 Original",
            Descripcion = "Jean clásico de corte recto",
            SKU = "JEAN-LEV-001",
            Precio = 65000m,
            Costo = 40000m,
            Stock = 15,
            CategoriaId = pantalones.IdCategoria,
            MarcaId = levis.IdMarca,
            ImagenUrl = "/img-productos/no-img.png",
            Activo = true,
            FechaCreacion = DateTime.Now
        },
        new Producto
        {
            Nombre = "Zapatillas Air Max",
            Descripcion = "Zapatillas running con amortiguación",
            SKU = "ZAP-NIKE-001",
            Precio = 120000m,
            Costo = 80000m,
            Stock = 10,
            CategoriaId = calzado.IdCategoria,
            MarcaId = nike.IdMarca,
            ImagenUrl = "/img-productos/no-img.png",
            Activo = true,
            FechaCreacion = DateTime.Now
        }
    };

            foreach (var p in productos)
            {
                if (!await context.Productos.AnyAsync(x => x.SKU == p.SKU))
                    context.Productos.Add(p);
            }

            await context.SaveChangesAsync();
        }
    }
}