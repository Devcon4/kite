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
	private readonly HttpRouteRouteOptions _httpRouteOptions;

	public KubernetesClient(Kubernetes client, IOptionsSnapshot<IngressOptions> ingressOptions, IOptionsSnapshot<IngressRouteOptions> ingressRouteOptions, IOptionsSnapshot<HttpRouteRouteOptions> httpRouteOptions) {
		_client = client;
		_ingressOptions = ingressOptions.Value;
		_ingressRouteOptions = ingressRouteOptions.Value;
		_httpRouteOptions = httpRouteOptions.Value;
	}

	public async Task<IEnumerable<Ingress>> GetIngresses() {
		if (!_ingressOptions.Enabled)
			return new List<Ingress>();

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
		if (!_ingressRouteOptions.Enabled)
			return new List<Ingress>();

		object ingresses = await _client.ListClusterCustomObjectAsync("traefik.containo.us", "v1alpha1", "ingressroutes", labelSelector: _ingressOptions?.LabelSelector);

		var converted = JsonSerializer.Deserialize(ingresses.ToString() ?? "", AppJsonSerializerContext.Default.IngressRouteList);
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

	public async Task<IEnumerable<Ingress>> GetHttpRoutes() {
		if (!_httpRouteOptions.Enabled)
			return new List<Ingress>();

		static string CalcPath(IEnumerable<string>? hostnames, string path, IDictionary<string, string>? annotations = null) {
			// If there is a specific path annotation, use that instead of the calculated one
			if (annotations != null && annotations.TryGetValue(Annotations.PATH, out var annotatedPath) && !string.IsNullOrWhiteSpace(annotatedPath)) {
				return annotatedPath;
			}

			if (hostnames == null || !hostnames.Any())
				return path;

			var first = hostnames.First();

			return $"https://{first}{(path.StartsWith("/") ? path : $"/{path}")}";
		}

		object ingresses = await _client.ListClusterCustomObjectAsync("gateway.networking.k8s.io", "v1beta1", "httproutes", labelSelector: _ingressOptions?.LabelSelector);
		var converted = JsonSerializer.Deserialize(ingresses.ToString() ?? "", AppJsonSerializerContext.Default.HttpRouteList);
		return converted?.items
			.Where(i => this._httpRouteOptions!.AnnotationSelector.Count() > 0 ? this._httpRouteOptions!.AnnotationSelector.All(a => i.metadata.annotations!.Any(ia => ia.Key == a.Key && ia.Value == a.Value)) : true)
			.Where(i => !i.metadata.annotations!.Where(a => a.Key == Annotations.ENABLED && a.Value == false.ToString()).Any())
			.SelectMany(i => i.spec.rules.Select(r => new Ingress(
				i.metadata.name,
				i.metadata.namespaceProperty,
				CalcPath(i.spec.hostnames, r.matches.First().path.value, i.metadata.annotations),
				"HttpRoute",
				i.metadata.annotations!.Where(a => a.Key.StartsWith(Annotations.BASE))
			))) ?? new List<Ingress>();

	}

}

// HttpRoute
public record HttpRouteList(IEnumerable<HttpRoute> items);
public record HttpRoute(HttpRouteMetadata metadata, HttpRouteSpec spec);
public class HttpRouteMetadata {
	public string? name { get; set; }
	[JsonPropertyName("namespace")]
	public string? namespaceProperty { get; set; }
	public IDictionary<string, string>? annotations { get; set; } = new Dictionary<string, string>();
};
public record HttpRouteSpec(IEnumerable<string> hostnames, IEnumerable<HttpRouteSpecRule> rules);
public record HttpRouteSpecRule(IEnumerable<HttpRouteSpecMatch> matches, IEnumerable<HttpRouteSpecBackendRefs> backendRefs);
public class HttpRouteSpecBackendRefs {
	public string? name { get; set; }
	[JsonPropertyName("namespace")]
	public string? namespaceProperty { get; set; }
	public int? port { get; set; }
	public int? weight { get; set; }
};
public record HttpRouteSpecMatch(HttpRouteSpecMatchPath path);
public record HttpRouteSpecMatchPath(string type, string value);

// IngressRoute
public record IngressRouteList(IEnumerable<IngressRoute> items);
public record IngressRoute(IngressRouteMetadata metadata, IngressRouteSpec spec);
public class IngressRouteMetadata {
	public string? name { get; set; }

	[JsonPropertyName("namespace")]
	public string? namespaceProperty { get; set; }
	public IDictionary<string, string>? annotations { get; set; } = new Dictionary<string, string>();
};

public record IngressRouteMetadataAnnotations();
public record IngressRouteSpec(IEnumerable<IngressRouteSpecRoute> routes);
public record IngressRouteSpecRoute(string match);
