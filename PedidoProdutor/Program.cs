using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Core.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetSection("MassTransit")["Server"], "/", h =>
        {
            h.Username(configuration.GetSection("MassTransit")["User"]);
            h.Password(configuration.GetSection("MassTransit")["Password"]);
        });
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnectionString"),
        sql => sql.EnableRetryOnFailure());
}, ServiceLifetime.Scoped);

builder.Services.AddScoped<IPedidoControleCozinhaRepository, PedidoControleCozinhaRepository>();
builder.Services.AddScoped<IPedidoItemRepository, PedidoItemRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Adiciona a validação automática e adaptadores de cliente
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Registro dos validadores
builder.Services.AddValidatorsFromAssemblyContaining<PedidoCancelationRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PedidoDeleteRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PedidoItemRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PedidoRequestValidator>();

builder.WebHost.UseUrls("http://*:8080");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseMetricServer();
app.UseHttpMetrics();
app.UseAuthorization();
app.MapControllers();
app.Run();

namespace PedidoProdutor
{
    public partial class Program { }
}

