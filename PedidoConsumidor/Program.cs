using PedidoConsumidor;
using PedidoConsumidor.Eventos;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var configuration = builder.Configuration;

var queueCadastroPedido = configuration.GetSection("MassTransit:Queues")["PedidoCadastroQueue"] ?? string.Empty;
var queueCancelamentoPedido = configuration.GetSection("MassTransit:Queues")["PedidoCancelamentoQueue"] ?? string.Empty;
var queueExclusaoPedido = configuration.GetSection("MassTransit:Queues")["PedidoExclusaoQueue"] ?? string.Empty;

builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoItemRepository, PedidoItemRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnectionString"),
        sql => sql.EnableRetryOnFailure());
}, ServiceLifetime.Scoped);

builder.Services.AddMassTransit(x =>
{

    x.AddConsumer<PedidoCriado>();
    x.AddConsumer<PedidoCancelado>();
    x.AddConsumer<PedidoDeletado>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetSection("MassTransit")["Server"], "/", h =>
        {
            h.Username(configuration.GetSection("MassTransit")["User"]);
            h.Password(configuration.GetSection("MassTransit")["Password"]);
        });

        cfg.ReceiveEndpoint(queueCadastroPedido, e =>
        {
            e.ConfigureDefaultDeadLetterTransport();
            e.ConfigureConsumer<PedidoCriado>(context);
        });

        cfg.ReceiveEndpoint(queueCancelamentoPedido, e =>
        {
            e.ConfigureDefaultDeadLetterTransport();
            e.ConfigureConsumer<PedidoCancelado>(context);
        });

        cfg.ReceiveEndpoint(queueExclusaoPedido, e =>
        {
            e.ConfigureDefaultDeadLetterTransport();
            e.ConfigureConsumer<PedidoDeletado>(context);
        });

        cfg.ConfigureEndpoints(context);
    });

   

});


var host = builder.Build();
host.Run();
