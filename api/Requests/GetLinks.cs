using Jackdaw.Core;
using Jackdaw.Interfaces;
using Kite.Domain;
using Kite.External;
using Kite.Mappers;
using Microsoft.Extensions.Options;

namespace Kite.Requests;

public record GetLinksResponse(List<Link> List, int Count) : IResponse;
public record GetLinksRequest : IRequest<GetLinksResponse> { }

public sealed class GetLinksHandler(KubernetesClient kubernetesClient, IOptionsSnapshot<StaticRouteOptions> staticRouteOptions) : IRequestHandler<GetLinksRequest, GetLinksResponse> {
	private readonly StaticRouteOptions _staticRouteOptions = staticRouteOptions.Value;

	public string IngressRoutePath(string? path) {
		if (path is null)
			return "";

		var parts = path.Split('&').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));
		var final = "";
		foreach (var part in parts) {
			var cleaned = part
			switch { { } when part.StartsWith("Host(`") => part.Replace("Host(`", "").Replace("`)", ""), { } when part.StartsWith("PathPrefix(`") => part.Replace("PathPrefix(`", "").Replace("`)", ""),
				_ => null
			};

			if (cleaned is not null) {
				final = $"{final}{cleaned}";
			}
		}

		return PathSchema(final);
	}

	public string HttpRoutePath(string? path) {
		if (path is null)
			return "";
		return path;
	}

	public string PathSchema(string? path) {
		if (path is null)
			return "";

		if (!(path.StartsWith("https://") || path.StartsWith("http://"))) {
			path = $"https://{path}";
		}

		return path;
	}

	public async Task<GetLinksResponse> Handle(GetLinksRequest request, CancellationToken cancellationToken) {
		var ingresses = (await kubernetesClient.GetIngresses()).Select(Mapper.IngressToLink).Select(i => i with { path = PathSchema(i.path) });
		var ingressRoutes = (await kubernetesClient.GetIngressRoutes()).Select(Mapper.IngressToLink).Select(i => i with { path = IngressRoutePath(i.path) });
		var httpRoutes = (await kubernetesClient.GetHttpRoutes()).Select(Mapper.IngressToLink).Select(i => i with { path = HttpRoutePath(i.path) });
		var staticRoutes = _staticRouteOptions.Enabled ? _staticRouteOptions.Routes.Select(Mapper.StaticRouteToLink).Select(i => i with { path = PathSchema(i.path) }) : new List<Link>();
		var routes = ingresses.Concat(ingressRoutes).Concat(httpRoutes).Concat(staticRoutes);
		return new GetLinksResponse(routes.ToList(), routes.Count());
	}

}
