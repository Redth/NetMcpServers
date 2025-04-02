using Microsoft.Deployment.DotNet.Releases;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MauiDevEnv;

[McpServerToolType]
public partial class DotnetTools
{
    [McpServerTool(Name = "dotnet_info")]
    [Description("Gets information about the installed .NET SDKs, runtimes, workloads, etc.")]
    public static async Task<string> GetDotNetInfo()
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet",
                Arguments = "--info",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var info = ParseDotnetInfoOutput(output);

        var releaseInfo = await GetDotNetReleasesInfo();

        info.LatestAvailableSdkVersion = releaseInfo?.ToString() ?? string.Empty;

		return JsonSerializer.Serialize(info, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        });
    }

    internal static async Task<ReleaseVersion?> GetDotNetReleasesInfo()
    {
        
        var products = await ProductCollection.GetAsync();

        var latest = products.Where(p => p.IsSupported && p.SupportPhase == SupportPhase.Active)
            .OrderByDescending(p => p.LatestSdkVersion)
            .FirstOrDefault();

        return latest?.LatestSdkVersion;
    }

    public static DotnetInfo ParseDotnetInfoOutput(string output)
    {
        var dotnetInfo = new DotnetInfo();
        
        // Split the output into lines
        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Define all possible section identifiers
        var sectionIdentifiers = new Dictionary<string, string>
        {
            { ".NET SDK:", "SDK" },
            { "Host:", "Host" },
            { "Runtime Environment:", "RuntimeEnvironment" },
            { ".NET workloads installed:", "Workloads" },
            { ".NET SDKs installed:", "SDKs" },
            { ".NET runtimes installed:", "Runtimes" },
            { "Other architectures found:", "OtherArchitectures" },
            { "Environment variables:", "EnvironmentVariables" },
            { "global.json file:", "GlobalJson" }
        };
        
        // First pass: Identify all section positions and their boundaries
        var sections = new Dictionary<string, (int start, int end)>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            
            // Check if this line is a section header
            if (sectionIdentifiers.TryGetValue(line, out var sectionType))
            {
                int sectionStart = i;
                int sectionEnd = lines.Length - 1; // Default to end of file
                
                // Find the end of this section (next section start or end of file)
                for (int j = i + 1; j < lines.Length; j++)
                {
                    if (sectionIdentifiers.Keys.Any(key => lines[j].Trim() == key))
                    {
                        sectionEnd = j - 1;
                        break;
                    }
                }
                
                sections[sectionType] = (sectionStart, sectionEnd);
            }
        }
        
        // Second pass: Process each section based on its type
        foreach (var section in sections)
        {
            string sectionType = section.Key;
            var (start, end) = section.Value;
            
            switch (sectionType)
            {
                case "SDK":
                    ProcessSdkSection(lines, start, end, dotnetInfo);
                    break;
                case "Host":
                    ProcessHostSection(lines, start, end, dotnetInfo);
                    break;
                case "RuntimeEnvironment":
                    ProcessRuntimeEnvironmentSection(lines, start, end, dotnetInfo);
                    break;
                case "Workloads":
                    ProcessWorkloadsSection(lines, start, end, dotnetInfo);
                    break;
                case "SDKs":
                    ProcessSdksSection(lines, start, end, dotnetInfo);
                    break;
                case "Runtimes":
                    ProcessRuntimesSection(lines, start, end, dotnetInfo);
                    break;
                case "OtherArchitectures":
                    ProcessOtherArchitecturesSection(lines, start, end, dotnetInfo);
                    break;
                case "EnvironmentVariables":
                    ProcessEnvironmentVariablesSection(lines, start, end, dotnetInfo);
                    break;
                case "GlobalJson":
                    ProcessGlobalJsonSection(lines, start, end, dotnetInfo);
                    break;
            }
        }
        
        return dotnetInfo;
    }
    
    private static void ProcessSdkSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                break;
                
            if (line.StartsWith("Version:"))
                dotnetInfo.Sdk.Version = ExtractValue(line);
            else if (line.StartsWith("Commit:"))
                dotnetInfo.Sdk.Commit = ExtractValue(line);
            else if (line.StartsWith("Workload version:"))
                dotnetInfo.Sdk.WorkloadVersion = ExtractValue(line);
            else if (line.StartsWith("MSBuild version:"))
                dotnetInfo.Sdk.MSBuildVersion = ExtractValue(line);
        }
    }
    
    private static void ProcessHostSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                break;
                
            if (line.StartsWith("Version:"))
                dotnetInfo.Host.Version = ExtractValue(line);
            else if (line.StartsWith("Architecture:"))
                dotnetInfo.Host.Architecture = ExtractValue(line);
            else if (line.StartsWith("Commit:"))
                dotnetInfo.Host.Commit = ExtractValue(line);
        }
    }
    
    private static void ProcessRuntimeEnvironmentSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                break;
                
            if (line.StartsWith("OS Name:"))
                dotnetInfo.RuntimeEnvironment.OSName = ExtractValue(line);
            else if (line.StartsWith("OS Version:"))
                dotnetInfo.RuntimeEnvironment.OSVersion = ExtractValue(line);
            else if (line.StartsWith("OS Platform:"))
                dotnetInfo.RuntimeEnvironment.OSPlatform = ExtractValue(line);
            else if (line.StartsWith("RID:"))
                dotnetInfo.RuntimeEnvironment.RID = ExtractValue(line);
            else if (line.StartsWith("Base Path:"))
                dotnetInfo.RuntimeEnvironment.BasePath = ExtractValue(line);
        }
    }
    
    private static void ProcessWorkloadsSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        WorkloadInfo? currentWorkload = null;
        
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            if (line.Contains("[") && line.Contains("]"))
            {
                // New workload
                currentWorkload = new WorkloadInfo();
                var match = Regex.Match(line, @"\[(.*?)\]");
                if (match.Success)
                {
                    currentWorkload.Name = match.Groups[1].Value;
                    dotnetInfo.Workloads.Add(currentWorkload);
                }
            }
            else if (currentWorkload != null)
            {
                if (line.StartsWith("Installation Source:"))
                    currentWorkload.InstallationSource = ExtractValue(line);
                else if (line.StartsWith("Manifest Version:"))
                    currentWorkload.ManifestVersion = ExtractValue(line);
                else if (line.StartsWith("Manifest Path:")) {
                    currentWorkload.ManifestPath = ExtractValue(line);

                    // Skip attempting to access the file system during parsing tests
                    try {
                        var manifestDir = Path.GetDirectoryName(currentWorkload.ManifestPath);
                        if (manifestDir != null)
                        {
                            var depPath = Path.Combine(manifestDir, "WorkloadDependencies.json");
                            if (File.Exists(depPath))
                            {
                                var depContent = File.ReadAllText(depPath);
                                var depJson = JsonDocument.Parse(depContent);
                                currentWorkload.WorkloadDependencies = depJson.RootElement;
                                currentWorkload.WorkloadDependenciesPath = depPath;
                            }
                        }
                    }
                    catch (Exception) {
                        // Ignore file system errors in tests or when files don't exist
                    }
                }
                else if (line.StartsWith("Install Type:"))
                    currentWorkload.InstallType = ExtractValue(line);
            }
        }
    }
    
    private static void ProcessSdksSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            var match = Regex.Match(line, @"([\d\.]+) \[(.*?)\]");
            if (match.Success)
            {
                dotnetInfo.InstalledSdks.Add(new SdkVersionInfo
                {
                    Version = match.Groups[1].Value,
                    Path = match.Groups[2].Value
                });
            }
        }
    }
    
    private static void ProcessRuntimesSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            var match = Regex.Match(line, @"(.*?) ([\d\.]+) \[(.*?)\]");
            if (match.Success)
            {
                dotnetInfo.InstalledRuntimes.Add(new RuntimeInfo
                {
                    Name = match.Groups[1].Value,
                    Version = match.Groups[2].Value,
                    Path = match.Groups[3].Value
                });
            }
        }
    }
    
    private static void ProcessOtherArchitecturesSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        for (int i = start + 1; i <= end; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            if (line.Contains("["))
            {
                var archMatch = Regex.Match(line, @"(.*?)\s+\[(.*?)\]");
                if (archMatch.Success)
                {
                    var archInfo = new ArchitectureInfo
                    {
                        Architecture = archMatch.Groups[1].Value.Trim(),
                        Path = archMatch.Groups[2].Value
                    };
                    
                    // Check for registry path in the next line
                    if (i + 1 <= end && lines[i + 1].Contains("registered at"))
                    {
                        var regMatch = Regex.Match(lines[i + 1], @"registered at \[(.*?)\]");
                        if (regMatch.Success)
                        {
                            archInfo.RegistryPath = regMatch.Groups[1].Value;
                            i++;
                        }
                    }
                    
                    dotnetInfo.OtherArchitectures.Add(archInfo);
                }
            }
        }
    }
    
    private static void ProcessEnvironmentVariablesSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        // Check if variables are set or not
        if (start + 1 <= end && lines[start + 1].Trim() != "Not set")
        {
            for (int i = start + 1; i <= end; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                    
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    dotnetInfo.EnvironmentVariables[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
    }
    
    private static void ProcessGlobalJsonSection(string[] lines, int start, int end, DotnetInfo dotnetInfo)
    {
        if (start + 1 <= end && lines[start + 1].Trim() != "Not found")
        {
            dotnetInfo.GlobalJsonPath = lines[start + 1].Trim();
        }
    }

    private static string ExtractValue(string line)
    {
        var parts = line.Split(new[] { ':' }, 2);
        return parts.Length > 1 ? parts[1].Trim() : string.Empty;
    }
}
