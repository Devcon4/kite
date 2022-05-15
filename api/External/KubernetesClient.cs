using k8s;
using Microsoft.Extensions.Options;

public interface IKubernetesFactory
{
  Kubernetes CreateClient();
}

public class KubernetesFactory : IKubernetesFactory
{
  public Kubernetes CreateClient()
  {
    var config = KubernetesClientConfiguration.BuildDefaultConfig();
    return new Kubernetes(config);
  }
}

public class IngressOptions {
  public string LabelSelector {get; init; } = String.Empty;
};

public record Ingress(string host, string path);

public class KubernetesClient {
  private readonly Kubernetes _client;
  private readonly IngressOptions _ingressOptions;

  KubernetesClient(Kubernetes client, IOptions<IngressOptions> ingressOptions) {
    _client = client;
    _ingressOptions = ingressOptions.Value ?? new IngressOptions();
  }

  public async Task<IEnumerable<Ingress>> GetIngresses() {

    var ingresses = await _client.ListIngressForAllNamespacesAsync(labelSelector: _ingressOptions?.LabelSelector);
    return ingresses.Items.SelectMany(i => i.Spec.Rules.Select(r => new Ingress($"{i.Metadata.NamespaceProperty ?? "Default"}/{i.Metadata.Name}", r.Host)));
  }
}
