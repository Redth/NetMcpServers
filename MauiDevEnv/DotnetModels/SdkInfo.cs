using System.Text.Json.Serialization;

namespace MauiDevEnv;
public partial class DotnetTools
{
	public class SdkInfo
    {
		[JsonPropertyName("version")]
		public string Version { get; set; } = string.Empty;

		[JsonPropertyName("commit")]
		public string Commit { get; set; } = string.Empty;

		[JsonPropertyName("workload_version")]
		public string WorkloadVersion { get; set; } = string.Empty;

		[JsonPropertyName("msbuild_version")]
		public string MSBuildVersion { get; set; } = string.Empty;
    }
}
