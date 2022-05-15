using System.Reflection;
using k8s;
using MediatR;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IngressOptions>(builder.Configuration.GetSection("Kite:Ingress"));
builder.Services.AddScoped(cfg => cfg.GetService<IOptions<IngressOptions>>()?.Value ?? new IngressOptions());

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<IKubernetesFactory, KubernetesFactory>();

builder.Services.AddScoped<Kubernetes>(s => s.GetRequiredService<IKubernetesFactory>().CreateClient());

builder.Services.AddScoped<KubernetesClient>();

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
