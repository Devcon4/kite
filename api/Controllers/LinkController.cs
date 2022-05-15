
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LinkController : ControllerBase
{
  private readonly IMediator _mediator;

  LinkController(IMediator mediator) {
    _mediator = mediator;
  }

  [HttpGet(Name = "getLinks")]
  public async Task<GetLinksResponse> GetLinks() {
    return await _mediator.Send(new GetLinksRequest());
  }
}