using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class SdkVersionInfo
    {
		[JsonPropertyName("version")]
		public string Version { get; set; } = string.Empty;

		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;
    }
}
