using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrdersManager.Contracts;
using OrdersManager.Data.Abstraction;
using OrdersManager.Data.Implementation;
using Order = OrdersManager.SharedModels.Order;
using OrderState = OrdersManager.SharedModels.OrderState;
using SubmitOrderConsumer = OrdersManager.Components.Consumers.SubmitOrderConsumer;
using OrderStateMachine = OrdersManager.Components.Consumers.OrderStateMachine;
using OrdersManager.Components.Consumers;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IOrderRepository<Order>, OrderRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

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
    x.AddConsumer<OrderPaymentConsumer>();

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
    dbContext.Database.EnsureCreated();
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
