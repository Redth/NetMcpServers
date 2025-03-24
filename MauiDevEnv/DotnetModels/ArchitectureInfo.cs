using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class ArchitectureInfo
    {
		[JsonPropertyName("architecture")]
		public string Architecture { get; set; } = string.Empty;

		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;

		[JsonPropertyName("registry_path")]
		public string RegistryPath { get; set; } = string.Empty;
    }
}
