namespace Kite.Domain;

public record SettingModel(string? AppName, IDictionary<string, int>? GroupOrder);
