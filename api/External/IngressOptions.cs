using System.Text.Json.Serialization;

namespace Kite.External;

public class IngressOptions {
  public bool Enabled { get; set; } = true;
  public string LabelSelector { get; set; } = String.Empty;
  public IDictionary<string, string> AnnotationSelector { get; set; } = new Dictionary<string, string>();
};

public class IngressRouteOptions {
  public bool Enabled { get; set; } = false;
  public string LabelSelector { get; set; } = String.Empty;
  public IDictionary<string, string> AnnotationSelector { get; set; } = new Dictionary<string, string>();
};

public class StaticRouteOptions {
  public bool Enabled { get; set; } = true;

  public List<StaticRoute> Routes { get; set; } = new List<StaticRoute>();
}

public class StaticRoute {
  public string Name { get; set; } = String.Empty;
  public string Path { get; set; } = String.Empty;

  [JsonPropertyName("Namespace")]
  public string NamespaceProperty { get; set; } = String.Empty;

  public string Kind { get; } = "Static";
  public IDictionary<string, string> Annotations { get; set; } = new Dictionary<string, string>();
}