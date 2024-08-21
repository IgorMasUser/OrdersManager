using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrdersManager.Components.Consumers;
using OrdersManager.Components.StateMachines;
using OrdersManager.Contracts;
using OrdersManager.Data.Abstraction;
using OrdersManager.Data.Implementation;
using OrdersManager.Models;
using static OrdersManager.Components.StateMachines.OrderStateMachine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IOrderRepository<Order>, OrderRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>(sagaConfig =>
    {
        sagaConfig.UseMessageRetry(r => r.Immediate(5));
        sagaConfig.UseInMemoryOutbox();
    })
    .EntityFrameworkRepository(r =>
    {
        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
        r.AddDbContext<DbContext, ApplicationDbContext>((provider, builder) =>
        {
            builder.UseSqlite("Data Source=orders.db");
        });
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
