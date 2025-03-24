using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class RuntimeInfo
    {
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("version")]
		public string Version { get; set; } = string.Empty;

		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;
    }
}
