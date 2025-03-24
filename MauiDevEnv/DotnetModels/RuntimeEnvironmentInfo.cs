using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class RuntimeEnvironmentInfo
    {
		[JsonPropertyName("os_name")]
		public string OSName { get; set; } = string.Empty;

		[JsonPropertyName("os_version")]
		public string OSVersion { get; set; } = string.Empty;

		[JsonPropertyName("os_platform")]
		public string OSPlatform { get; set; } = string.Empty;

		[JsonPropertyName("runtime_identifier")]
		public string RID { get; set; } = string.Empty;

		[JsonPropertyName("base_path")]
		public string BasePath { get; set; } = string.Empty;
    }
}
