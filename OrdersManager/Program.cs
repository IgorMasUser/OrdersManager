using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrdersManager.Components.Consumers;
using OrdersManager.Components.StateMachines;
using OrdersManager.Contracts;
using OrdersManager.Data.Abstraction;
using OrdersManager.Data.Implementation;
using OrdersManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IOrderRepository<Order>, OrderRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>(sagaConfig =>
    {
        sagaConfig.UseMessageRetry(r => r.Immediate(3));
        sagaConfig.UseInMemoryOutbox();
    })
    .EntityFrameworkRepository(r =>
    {
        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
        r.ExistingDbContext<ApplicationDbContext>();

        r.UseSqlServer();
    });

    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<SubmitOrderConsumer>();

    x.AddRequestClient<SubmitOrder>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
    x.AddRequestClient<CheckOrder>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var pendingMigrations = dbContext.Database.GetPendingMigrations();

    if (pendingMigrations.Any())
    {
        dbContext.Database.Migrate();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
