using System.Reflection;
using k8s;
using Kite.External;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
  .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "appsettings.json"), optional : true, reloadOnChange : true)
  .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "secrets/appsettings.json"), optional : true, reloadOnChange : true)
  .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional : true, reloadOnChange : true)
  .AddEnvironmentVariables();

builder.Services.Configure<IngressOptions>(builder.Configuration.GetSection("Kite:Ingress"));
builder.Services.Configure<IngressRouteOptions>(builder.Configuration.GetSection("Kite:IngressRoute"));
builder.Services.Configure<StaticRouteOptions>(builder.Configuration.GetSection("Kite:StaticRoute"));
builder.Services.Configure<SettingOptions>(builder.Configuration.GetSection("Kite:Settings"));

builder.Services.AddScoped<IKubernetesFactory, KubernetesFactory>();

builder.Services.AddScoped<Kubernetes>(s => s.GetRequiredService<IKubernetesFactory>().CreateClient());
builder.Services.AddScoped<KubernetesClient>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();