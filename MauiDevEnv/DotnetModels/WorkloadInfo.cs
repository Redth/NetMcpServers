using System.Text.Json;
using System.Text.Json.Serialization;

namespace MauiDevEnv;

public partial class DotnetTools
{
	public class WorkloadInfo
    {
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("installation_source")]
		public string InstallationSource { get; set; } = string.Empty;

		[JsonPropertyName("manifest_version")]
		public string ManifestVersion { get; set; } = string.Empty;

		[JsonPropertyName("workload_manifest_path")]
		public string ManifestPath { get; set; } = string.Empty;

		[JsonPropertyName("workload_dependencies_path")]
		public string WorkloadDependenciesPath { get; set; } = string.Empty;

		[JsonPropertyName("workload_dependencies")]
		public JsonElement? WorkloadDependencies { get; set; }

		[JsonPropertyName("install_type")]
		public string InstallType { get; set; } = string.Empty;
    }
}
