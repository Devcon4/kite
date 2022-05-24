using Kite.Domain;
using Kite.External;
using MediatR;
using Microsoft.Extensions.Options;

namespace Kite.Requests;

public record GetLinksResponse(List<Link> List, int Count);
public record GetLinksRequest : IRequest<GetLinksResponse> { };

public class GetLinksHandler : IRequestHandler<GetLinksRequest, GetLinksResponse> {
  private readonly KubernetesClient _kubernetesClient;
  private readonly StaticRouteOptions _staticRouteOptions;

  public GetLinksHandler(KubernetesClient kubernetesClient, IOptionsMonitor<StaticRouteOptions> staticRouteOptions) {
    _kubernetesClient = kubernetesClient;
    _staticRouteOptions = staticRouteOptions.CurrentValue;
  }
  public string IngressRoutePath(string? path) {
    if (path is null) return "";

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

  public string PathSchema(string? path) {
    if (path is null) return "";

    if (!(path.StartsWith("https://") || path.StartsWith("http://"))) {
      path = $"https://{path}";
    }

    return path;
  }

  public async Task<GetLinksResponse> Handle(GetLinksRequest request, CancellationToken cancellationToken) {

    var ingresses = (await _kubernetesClient.GetIngresses()).Select(Mappers.IngressToLink).Select(i => i with { path = PathSchema(i.path) });
    var ingressRoutes = (await _kubernetesClient.GetIngressRoutes()).Select(Mappers.IngressToLink).Select(i => i with { path = IngressRoutePath(i.path) });
    var staticRoutes = _staticRouteOptions.Enabled ? _staticRouteOptions.Routes.Select(Mappers.StaticRouteToLink).Select(i => i with { path = PathSchema(i.path) }) : new List<Link>();
    var routes = ingresses.Concat(ingressRoutes).Concat(staticRoutes);
    return new GetLinksResponse(routes.ToList(), routes.Count());
  }
}