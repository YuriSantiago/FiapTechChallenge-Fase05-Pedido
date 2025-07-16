using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                SeedDatabase(db).Wait();
            });

        }

        private static async Task SeedDatabase(ApplicationDbContext context)
        {

            context.Pedidos.RemoveRange(context.Pedidos);
            context.Usuarios.RemoveRange(context.Usuarios);

            await context.SaveChangesAsync();

            var usuario = context.Usuarios.Add(new Usuario
            {
                Nome = "Eduardo",
                Email = "eduardo@email.com",
                Senha = Base64Helper.Encode("eduardo"),
                Role = "CLIENTE"
            });

            await context.SaveChangesAsync();

            context.Pedidos.Add(new Pedido
            {
                DataInclusao = DateTime.Now,
                UsuarioId = 1,
                PrecoTotal = 50.00M,
                Status = StatusPedido.Pendente,
                TipoEntrega = "DELIVERY",
                Usuario = usuario.Entity
            });

            context.Pedidos.Add(new Pedido
            {
                DataInclusao = DateTime.Now,
                UsuarioId = 1,
                PrecoTotal = 60.00M,
                Status = StatusPedido.Aceito,
                TipoEntrega = "DELIVERY",
                Usuario = usuario.Entity
            });

            await context.SaveChangesAsync();
        }

    }
}
