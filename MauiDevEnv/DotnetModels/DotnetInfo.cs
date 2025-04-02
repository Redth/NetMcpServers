using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class DotnetInfo
    {
        [JsonPropertyName("latest_available_sdk_version")]
        public string? LatestAvailableSdkVersion { get; set; }

		[JsonPropertyName("sdk_info")]
		public SdkInfo Sdk { get; set; } = new SdkInfo();

		[JsonPropertyName("runtime_environment_info")]
		public RuntimeEnvironmentInfo RuntimeEnvironment { get; set; } = new RuntimeEnvironmentInfo();

		[JsonPropertyName("workloads_info")]
		public List<WorkloadInfo> Workloads { get; set; } = new List<WorkloadInfo>();

		[JsonPropertyName("host_info")]
		public HostInfo Host { get; set; } = new HostInfo();

		[JsonPropertyName("installed_sdks_info")]
		public List<SdkVersionInfo> InstalledSdks { get; set; } = new List<SdkVersionInfo>();

		[JsonPropertyName("installed_runtimes_info")]
		public List<RuntimeInfo> InstalledRuntimes { get; set; } = new List<RuntimeInfo>();

		[JsonPropertyName("architecture_info")]
		public List<ArchitectureInfo> OtherArchitectures { get; set; } = new List<ArchitectureInfo>();

		[JsonPropertyName("environment_variables")]
		public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

		[JsonPropertyName("global_json")]
		public string GlobalJsonPath { get; set; } = string.Empty;
    }
}
