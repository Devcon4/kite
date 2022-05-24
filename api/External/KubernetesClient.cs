using System.Text.Json;
using System.Text.Json.Serialization;
using k8s;
using Microsoft.Extensions.Options;

namespace Kite.External;

public interface IKubernetesFactory {
  Kubernetes CreateClient();
}

public class KubernetesFactory : IKubernetesFactory {
  public Kubernetes CreateClient() {
    var config = KubernetesClientConfiguration.BuildDefaultConfig();
    return new Kubernetes(config);
  }
}

public record Ingress(string? Host, string? NamespaceProperty, string? Path, string Kind, IEnumerable<KeyValuePair<string, string>> Annotations);

public class KubernetesClient {
  private readonly Kubernetes _client;
  private readonly IngressOptions _ingressOptions;
  private readonly IngressRouteOptions _ingressRouteOptions;

  public KubernetesClient(Kubernetes client, IOptionsMonitor<IngressOptions> ingressOptions, IOptionsMonitor<IngressRouteOptions> ingressRouteOptions) {
    _client = client;
    _ingressOptions = ingressOptions.CurrentValue;
    _ingressRouteOptions = ingressRouteOptions.CurrentValue;
  }

  public async Task<IEnumerable<Ingress>> GetIngresses() {

    var ingresses = await _client.ListIngressForAllNamespacesAsync(labelSelector: _ingressOptions?.LabelSelector);
    return ingresses.Items
      .Where(i => this._ingressOptions!.AnnotationSelector.Count() > 0 ? this._ingressOptions!.AnnotationSelector.All(a => i.Metadata.Annotations.Any(ia => ia.Key == a.Key && ia.Value == a.Value)) : true)
      .Where(i => !i.Metadata.Annotations!.Where(a => a.Key == Annotations.ENABLED && a.Value == false.ToString()).Any())
      .SelectMany(i => i.Spec.Rules.Select(r => new Ingress(
        i.Metadata.Name,
        i.Metadata.NamespaceProperty,
        r.Host,
        "Ingress",
        i.Metadata.Annotations.Where(a => a.Key.StartsWith(Annotations.BASE))
      ))) ?? new List<Ingress>();
  }

  public async Task<IEnumerable<Ingress>> GetIngressRoutes() {
    if (!_ingressRouteOptions.Enabled) return new List<Ingress>();

    object ingresses = await _client.ListClusterCustomObjectAsync("traefik.containo.us", "v1alpha1", "ingressroutes", labelSelector : _ingressOptions?.LabelSelector);

    var converted = JsonSerializer.Deserialize<IngressRouteList>(ingresses.ToString() ?? "");
    return converted?.items
      .Where(i => this._ingressRouteOptions!.AnnotationSelector.Count() > 0 ? this._ingressRouteOptions!.AnnotationSelector.All(a => i.metadata.annotations!.Any(ia => ia.Key == a.Key && ia.Value == a.Value)) : true)
      .Where(i => !i.metadata.annotations!.Where(a => a.Key == Annotations.ENABLED && a.Value == false.ToString()).Any())
      .SelectMany(i => i.spec.routes.Select(r => new Ingress(
        i.metadata.name,
        i.metadata.namespaceProperty,
        r.match,
        "IngressRoute",
        i.metadata.annotations!.Where(a => a.Key.StartsWith(Annotations.BASE))
      ))) ?? new List<Ingress>();
  }
}

public record IngressRouteList(IEnumerable<IngressRoute> items);
public record IngressRoute(IngressRouteMetadata metadata, IngressRouteSpec spec);
public class IngressRouteMetadata {
  public string? name { get; set; }

  [JsonPropertyName("namespace")]
  public string? namespaceProperty { get; set; }
  public IDictionary<string, string> ? annotations { get; set; } = new Dictionary<string, string>();
};

public record IngressRouteMetadataAnnotations();
public record IngressRouteSpec(IEnumerable<IngressRouteSpecRoute> routes);
public record IngressRouteSpecRoute(string match);