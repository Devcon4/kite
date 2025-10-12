using Jackdaw.Core;
using Jackdaw.Interfaces;
using Kite.Domain;
using Kite.Mappers;
using Microsoft.Extensions.Options;

namespace Kite.Requests;

public record GetSettingsResponse(SettingModel settings) : IResponse;

public record GetSettingsRequest : IRequest<GetSettingsResponse> { }

public sealed class GetSettingsHandler(IOptionsMonitor<SettingOptions> settings) : IRequestHandler<GetSettingsRequest, GetSettingsResponse> {

	public Task<GetSettingsResponse> Handle(GetSettingsRequest request, CancellationToken cancellationToken) {
		return Task.FromResult(new GetSettingsResponse(Mapper.SettingOptionsToSettingModel(settings.CurrentValue)));
	}
}
