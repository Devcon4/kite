namespace Kite.Domain;

public record Link(
	string? path,
	string? name,
	string? namespaceProperty,
	string kind,
	IEnumerable<string>? tags, string? group,
	string? description
);
