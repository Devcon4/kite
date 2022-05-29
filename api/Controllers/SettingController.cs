using Kite.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kite.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingController : ControllerBase {
  private readonly IMediator _mediator;

  public SettingController(IMediator mediator) {
    _mediator = mediator;
  }

  [HttpGet(Name = "getSettings")]
  public async Task<GetSettingsResponse> GetSettings() {
    return await _mediator.Send(new GetSettingsRequest());
  }
}