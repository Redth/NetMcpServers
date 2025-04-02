using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Common;
using NuGet.Configuration;
using System.Text.RegularExpressions;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace NetMcp.NuGet;

[McpServerToolType]
public partial class NuGetTools
{
    /// <summary>
    /// The default NuGet.org package source URL
    /// </summary>
    public const string DefaultNuGetSource = "https://api.nuget.org/v3/index.json";

    [McpServerTool(Name ="nuget_search")]
    [Description(NuGetSearchDescription)]
    public static async Task<string> SearchPackagesAsync(
        [Description(NuGetSearchQueryDescription)]
        string query,
        [Description("A semi-colon delimited list of NuGet API URLs to search. If not provided, defaults to the v3 NuGet.org API url.")]
        string? nuget_sources = null,
        [Description("Whether to include prerelease packages in the results (default: false)")]
        bool allow_prerelease = false,
        [Description("The number of results to skip (for pagination, default: 0)")]
        int skip = 0,
        [Description("The maximum number of results to return (default: 30)")]
        int take = 30
        )
    {
        var ns = nuget_sources?.Split(';').Select(s => s.Trim()).ToArray();

        // Use default NuGet source if none specified
        var sources = ns?.Any() == true
            ? ns
            : new[] { DefaultNuGetSource };

        var results = new List<IPackageSearchMetadata>();
        
        foreach (var sourceUrl in sources)
        {
            try
            {
                // Set up the package source repository
                var packageSource = new PackageSource(sourceUrl);
                var repository = Repository.Factory.GetCoreV3(packageSource);
                
                // Get the search resource
                var searchResource = await repository.GetResourceAsync<PackageSearchResource>(default);
                
                // Search for packages
                var searchFilter = new SearchFilter(allow_prerelease);
                var searchResults = await searchResource.SearchAsync(
                    query,
                    searchFilter,
                    skip,
                    take,
                    NullLogger.Instance,
                    default);
                    
                results.AddRange(searchResults);
            }
            catch (Exception ex)
            {
                await System.Console.Error.WriteLineAsync($"Error searching NuGet source {sourceUrl}: {ex.Message}");
                // Continue with other sources if one fails
            }
        }

        return results.ToArray().ToJson();
    }
    
    /// <summary>
    /// Converts a glob pattern to a regex pattern and checks if the file path matches
    /// </summary>
    private static bool IsMatch(string filePath, string pattern)
    {
        // Convert glob pattern to regex pattern
        string regexPattern = "^" + Regex.Escape(pattern)
            .Replace("\\*\\*", ".*")           // ** matches any number of directories
            .Replace("\\*", "[^/\\\\]*")       // * matches any chars except path separators
            .Replace("\\?", "[^/\\\\]")        // ? matches a single non-path-separator char
            + "$";
            
        return Regex.IsMatch(filePath, regexPattern, RegexOptions.IgnoreCase);
    }
}
