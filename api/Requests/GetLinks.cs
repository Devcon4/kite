using MediatR;

public record GetLinksResponse(List<Link> List, int Count);
public record GetLinksRequest: IRequest<GetLinksResponse>{};

public class GetLinksHandler : IRequestHandler<GetLinksRequest, GetLinksResponse> {
  private readonly KubernetesClient _kubernetesClient;

  GetLinksHandler(KubernetesClient kubernetesClient) {
    _kubernetesClient = kubernetesClient;
  }

  public async Task<GetLinksResponse> Handle(GetLinksRequest request, CancellationToken cancellationToken) {

    var ingresses = await _kubernetesClient.GetIngresses();
    return new GetLinksResponse(ingresses.Select(i => new Link(i.path, i.host)).ToList(), ingresses.Count());
  }
}