using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class HostInfo
    {
		[JsonPropertyName("version")]
		public string Version { get; set; } = string.Empty;

		[JsonPropertyName("architecture")]
		public string Architecture { get; set; } = string.Empty;

		[JsonPropertyName("commit")]
		public string Commit { get; set; } = string.Empty;
    }
}
