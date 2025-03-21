using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Packaging.Core;
using NuGet.Packaging;  // Add this using statement

namespace NetMcp.NuGet
{
    internal class NuGetUtil
    {
        /// <summary>
        /// Gets the latest version of a NuGet package from the specified source.
        /// </summary>
        /// <param name="packageId">The ID of the package to check.</param>
        /// <param name="includePrerelease">Whether to include prerelease versions (default: false).</param>
        /// <param name="sourceUrl">The NuGet source URL (default: nuget.org).</param>
        /// <returns>The latest version of the package, or null if not found.</returns>
        public static async Task<NuGetVersion?> GetLatestPackageVersionAsync(
            string packageId,
            bool includePrerelease = false,
            string? sourceUrl = null)
        {
            sourceUrl ??= NuGetTools.DefaultNuGetSource;

            try
            {
                // Configure the package source
                var packageSource = new PackageSource(sourceUrl);
                var repository = Repository.Factory.GetCoreV3(packageSource);
                
                // Get the package metadata resource
                var packageMetadataResource = await repository.GetResourceAsync<PackageMetadataResource>();
                
                // Get package metadata and find latest version
                var metadata = await packageMetadataResource.GetMetadataAsync(
                    packageId,
                    includePrerelease: includePrerelease,
                    includeUnlisted: false,
                    sourceCacheContext: new SourceCacheContext(),
                    log: NullLogger.Instance,
                    token: default);
                    
                return metadata
                    .OrderByDescending(m => m.Identity.Version)
                    .Select(m => m.Identity.Version)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to get latest version for package '{packageId}' from {sourceUrl}: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        /// Downloads a NuGet package as a byte array.
        /// </summary>
        /// <param name="packageId">The ID of the package to download.</param>
        /// <param name="version">The specific version to download, or null for latest version.</param>
        /// <param name="includePrerelease">Whether to include prerelease versions when resolving latest version.</param>
        /// <param name="sourceUrl">The NuGet source URL (default: nuget.org).</param>
        /// <returns>The package contents as a byte array.</returns>
        public static async Task<byte[]> GetPackageBytesAsync(
            string packageId,
            string? version = null,
            bool includePrerelease = false,
            string? sourceUrl = null)
        {
            sourceUrl ??= NuGetTools.DefaultNuGetSource;

            try
            {
                // Configure the package source
                var packageSource = new PackageSource(sourceUrl);
                var repository = Repository.Factory.GetCoreV3(packageSource);
                
                // Get required resources
                var downloadResource = await repository.GetResourceAsync<DownloadResource>();

                // Resolve version if not specified
                NuGetVersion nugetVersion;
                if (version == null)
                {
                    nugetVersion = await GetLatestPackageVersionAsync(packageId, includePrerelease, sourceUrl) 
                        ?? throw new InvalidOperationException($"Package '{packageId}' not found.");
                }
                else if (!NuGetVersion.TryParse(version, out nugetVersion!))
                {
                    throw new ArgumentException($"Invalid package version format: {version}", nameof(version));
                }

                // Download the package
                var packageIdentity = new PackageIdentity(packageId, nugetVersion);
                var context = new PackageDownloadContext(new SourceCacheContext());
                var result = await downloadResource.GetDownloadResourceResultAsync(
                    packageIdentity,
                    context,
                    Path.GetTempPath(),
                    NullLogger.Instance,
                    default);

                if (result.Status != DownloadResourceResultStatus.Available)
                {
                    throw new InvalidOperationException(
                        $"Package '{packageId}' version {nugetVersion} download failed with status: {result.Status}");
                }

                // Convert package stream to byte array
                using var memoryStream = new MemoryStream();
                await result.PackageStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"Failed to download package '{packageId}' version {version ?? "latest"} from {sourceUrl}: {ex.Message}",
                    ex);
            }
        }

        /// <summary>
        /// Gets the contents of a specific file from a NuGet package as a byte array.
        /// </summary>
        /// <param name="packageId">The ID of the package.</param>
        /// <param name="filePath">The path of the file within the package.</param>
        /// <param name="version">The specific version to use, or null for latest version.</param>
        /// <param name="includePrerelease">Whether to include prerelease versions when resolving latest version.</param>
        /// <param name="sourceUrl">The NuGet source URL (default: nuget.org).</param>
        /// <returns>The file contents as a byte array.</returns>
        public static async Task<byte[]> GetPackageFileAsync(
            string packageId,
            string filePath,
            string? version = null,
            bool includePrerelease = false,
            string? sourceUrl = null)
        {
            sourceUrl ??= NuGetTools.DefaultNuGetSource;

            try
            {
                // Configure the package source
                var packageSource = new PackageSource(sourceUrl);
                var repository = Repository.Factory.GetCoreV3(packageSource);
                
                // Get required resources
                var downloadResource = await repository.GetResourceAsync<DownloadResource>();

                // Resolve version if not specified
                NuGetVersion nugetVersion;
                if (version == null)
                {
                    nugetVersion = await GetLatestPackageVersionAsync(packageId, includePrerelease, sourceUrl) 
                        ?? throw new InvalidOperationException($"Package '{packageId}' not found.");
                }
                else if (!NuGetVersion.TryParse(version, out nugetVersion!))
                {
                    throw new ArgumentException($"Invalid package version format: {version}", nameof(version));
                }

                // Download the package
                var packageIdentity = new PackageIdentity(packageId, nugetVersion);
                var context = new PackageDownloadContext(new SourceCacheContext());
                using var result = await downloadResource.GetDownloadResourceResultAsync(
                    packageIdentity,
                    context,
                    Path.GetTempPath(),
                    NullLogger.Instance,
                    default);

                if (result.Status != DownloadResourceResultStatus.Available)
                {
                    throw new InvalidOperationException(
                        $"Package '{packageId}' version {nugetVersion} download failed with status: {result.Status}");
                }

                // Use PackageArchiveReader directly with the stream
                using var packageReader = new PackageArchiveReader(result.PackageStream);
                
                // Get all files in the package
                var packageFiles = packageReader.GetFiles().ToList();
                
                // Check if the requested file exists
                if (!packageFiles.Contains(filePath))
                {
                    throw new FileNotFoundException(
                        $"File '{filePath}' not found in package {packageId}" +
                        $" version {nugetVersion}. Available files: {string.Join(", ", packageFiles)}");
                }

                // Extract the specific file
                using var fileStream = packageReader.GetStream(filePath);
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                
                var d = memoryStream.ToArray();
                return d;
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not FileNotFoundException)
            {
                throw new InvalidOperationException(
                    $"Failed to extract file '{filePath}' from package '{packageId}' version {version ?? "latest"} from {sourceUrl}: {ex.Message}",
                    ex);
            }
        }
    }
}
