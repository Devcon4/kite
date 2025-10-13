using Kite.Domain;
using Kite.External;

namespace Kite.Mappers;

public static partial class Mapper {

	private static string GetAnnotation(string annotationKey, IEnumerable<KeyValuePair<string, string>> annotations, string defaultValue = "") =>
		annotations
		.Where(a => a.Key == annotationKey)
		.Select(a => a.Value)
		.FirstOrDefault(defaultValue);

	public static Func<Ingress, Link> IngressToLink => i => new Link(
		GetAnnotation(Annotations.PATH, i.Annotations, i.Path ?? ""),
		GetAnnotation(Annotations.NAME, i.Annotations, i.Host ?? ""),
		i.NamespaceProperty,
		i.Kind,
		GetAnnotation(Annotations.TAGS, i.Annotations).ToLower().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
		GetAnnotation(Annotations.GROUP, i.Annotations),
		GetAnnotation(Annotations.DESCRIPTION, i.Annotations));
	public static Func<StaticRoute, Link> StaticRouteToLink => r => new Link(
		GetAnnotation(Annotations.PATH, r.Annotations, r.Path),
		GetAnnotation(Annotations.NAME, r.Annotations, r.Name),
		r.NamespaceProperty,
		r.Kind,
		GetAnnotation(Annotations.TAGS, r.Annotations).ToLower().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
		GetAnnotation(Annotations.GROUP, r.Annotations),
		GetAnnotation(Annotations.DESCRIPTION, r.Annotations));
}
