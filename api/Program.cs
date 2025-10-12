using Jackdaw.Core;
using Jackdaw.Interfaces;
using k8s;
using Kite;
using Kite.External;
using Kite.Requests;

var builder = WebApplication.CreateSlimBuilder(args);

// Add services to the container.
builder.Services.ConfigureHttpJsonOptions(options => {
	options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Configuration
	.AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: true)
	.AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "secrets/appsettings.json"), optional: true, reloadOnChange: true)
	.AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true)
	.AddEnvironmentVariables();

builder.Services.AddOptionsWithValidateOnStart<IngressOptions>().Bind(builder.Configuration.GetSection("Kite:Ingress"));
builder.Services.AddOptionsWithValidateOnStart<IngressRouteOptions>().Bind(builder.Configuration.GetSection("Kite:IngressRoute"));
builder.Services.AddOptionsWithValidateOnStart<StaticRouteOptions>().Bind(builder.Configuration.GetSection("Kite:StaticRoute"));
builder.Services.AddOptionsWithValidateOnStart<SettingOptions>().Bind(builder.Configuration.GetSection("Kite:Settings"));

builder.Services.AddSingleton<IKubernetesFactory, KubernetesFactory>();

builder.Services.AddSingleton<Kubernetes>(s => s.GetRequiredService<IKubernetesFactory>().CreateClient());
builder.Services.AddSingleton<KubernetesClient>();

// Register jackdaw
builder.Services.AddJackdaw(opts => {
	opts.UseInMemoryQueue();
});

Console.WriteLine($"Jackdaw registered handlers");
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions {
	OnPrepareResponse = ctx => {
		var time = 60 * 60 * 24 * 30; // 30 days
		ctx.Context.Response.Headers.CacheControl = "public,max-age=" + time.ToString("0");
	}
});

// Minimal API endpoints
var apiGroup = app.MapGroup("/api");

apiGroup.MapGet("/link", async (IMediator mediator) => {
	return await mediator.Send(new GetLinksRequest());
})
.WithName("getLinks")
.Produces<GetLinksResponse>();

apiGroup.MapGet("/setting", async (IMediator mediator) => {
	return await mediator.Send(new GetSettingsRequest());
})
.WithName("getSettings")
.Produces<GetSettingsResponse>();

app.MapFallbackToFile("index.html");

Console.WriteLine("Starting app");
app.Run();
