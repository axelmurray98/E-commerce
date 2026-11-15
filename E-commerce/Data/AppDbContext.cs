using E_commerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
       : IdentityDbContext<AppUsuario>(options)
    {
        public DbSet<AppUsuario> AppUsuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }

        public DbSet<Direccion> Direcciones { get; set; }

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoDetalle> CarritoDetalles { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }

        public DbSet<MovimientoStock> MovimientosStock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(p => p.IdProducto);
                entity.Property(p => p.Nombre).IsRequired();
                entity.Property(p => p.SKU).IsRequired();
                entity.HasIndex(p => p.SKU).IsUnique();
                entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Costo).HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(p => p.CategoriaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Marca)
                    .WithMany(m => m.Productos)
                    .HasForeignKey(p => p.MarcaId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Las relaciones con CarritoDetalle, PedidoDetalle y MovimientoStock
                // se definen más abajo, del lado de cada detalle (una sola vez).
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(c => c.IdCategoria);
                entity.Property(c => c.Nombre).IsRequired();
            });

            modelBuilder.Entity<Marca>(entity =>
            {
                entity.HasKey(m => m.IdMarca);
                entity.Property(m => m.Nombre).IsRequired();
            });

            modelBuilder.Entity<Direccion>(entity =>
            {
                entity.HasKey(d => d.DireccionId);
                entity.Property(d => d.UsuarioId).IsRequired();

                entity.HasOne(d => d.Usuario)
                    .WithMany(u => u.Direcciones)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Carrito>(entity =>
            {
                entity.HasKey(c => c.CarritoId);
                entity.Property(c => c.UsuarioId).IsRequired();

                entity.HasOne(c => c.Usuario)
                    .WithOne(u => u.Carrito)
                    .HasForeignKey<Carrito>(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CarritoDetalle>(entity =>
            {
                entity.HasKey(cd => cd.CarritoDetalleId);
                entity.Property(cd => cd.Cantidad).IsRequired();

                // Carrito 1 ── N CarritoDetalle
                entity.HasOne(cd => cd.Carrito)
                    .WithMany(c => c.Detalles)
                    .HasForeignKey(cd => cd.CarritoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Producto 1 ── N CarritoDetalle
                entity.HasOne(cd => cd.Producto)
                    .WithMany(p => p.Carritos)
                    .HasForeignKey(cd => cd.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.PedidoId);
                entity.Property(p => p.UsuarioId).IsRequired();
                entity.Property(p => p.Calle).IsRequired();
                entity.Property(p => p.Numero).IsRequired();
                entity.Property(p => p.Ciudad).IsRequired();
                entity.Property(p => p.Provincia).IsRequired();
                entity.Property(p => p.CodigoPostal).IsRequired();

                entity.HasOne(p => p.Usuario)
                    .WithMany(u => u.Pedidos)
                    .HasForeignKey(p => p.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.HasKey(pd => pd.PedidoDetalleId);
                entity.Property(pd => pd.NombreProducto).IsRequired();
                entity.Property(pd => pd.PrecioUnitario).HasColumnType("decimal(18,2)");
                entity.Property(pd => pd.Subtotal).HasColumnType("decimal(18,2)");

                entity.HasOne<Pedido>()
                    .WithMany()
                    .HasForeignKey(pd => pd.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Restrict: un pedido histórico no debe desaparecer si borrás el producto
                entity.HasOne<Producto>()
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(pd => pd.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MovimientoStock>(entity =>
            {
                entity.HasKey(m => m.MovimientoStockId);
                entity.Property(m => m.Motivo).IsRequired();

                // Restrict: el historial de stock se conserva
                entity.HasOne<Producto>()
                    .WithMany(p => p.MovimientosStock)
                    .HasForeignKey(m => m.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
