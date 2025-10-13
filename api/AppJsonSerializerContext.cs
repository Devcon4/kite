using System.Text.Json.Serialization;
using Kite.Domain;
using Kite.External;
using Kite.Requests;

namespace Kite;

[JsonSourceGenerationOptions(
		PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		WriteIndented = false)]
[JsonSerializable(typeof(GetLinksResponse))]
[JsonSerializable(typeof(GetSettingsResponse))]
[JsonSerializable(typeof(Link))]
[JsonSerializable(typeof(SettingModel))]
[JsonSerializable(typeof(List<Link>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
[JsonSerializable(typeof(IngressRouteList))]
[JsonSerializable(typeof(IngressRoute))]
[JsonSerializable(typeof(IngressRouteMetadata))]
[JsonSerializable(typeof(IngressRouteSpec))]
[JsonSerializable(typeof(IngressRouteSpecRoute))]
[JsonSerializable(typeof(HttpRouteList))]
[JsonSerializable(typeof(StaticRouteOptions))]
[JsonSerializable(typeof(IngressList))]
public partial class AppJsonSerializerContext : JsonSerializerContext { }
