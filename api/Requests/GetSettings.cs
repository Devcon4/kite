using Kite.Domain;
using Kite.Mappers;
using MediatR;
using Microsoft.Extensions.Options;

namespace Kite.Requests;

public record GetSettingsResponse(SettingModel settings);

public record GetSettingsRequest : IRequest<GetSettingsResponse> { };
public class GetSettingsHandler : IRequestHandler<GetSettingsRequest, GetSettingsResponse> {
  private readonly IOptionsMonitor<SettingOptions> _settings;

  public GetSettingsHandler(IOptionsMonitor<SettingOptions> settings) {
    _settings = settings;
  }

  public Task<GetSettingsResponse> Handle(GetSettingsRequest request, CancellationToken cancellationToken) {
    return Task.FromResult(new GetSettingsResponse(Mapper.SettingOptionsToSettingModel(_settings.CurrentValue)));
  }
}