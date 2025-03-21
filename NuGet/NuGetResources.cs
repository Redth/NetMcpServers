using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using NuGet.Versioning;

namespace NetMcp.NuGet;

public static class NuGetResources
{
	internal static Task<ListResourcesResult> ListResourcesHandler(RequestContext<ListResourcesRequestParams> context, CancellationToken token)
		=> Task.FromResult(new ListResourcesResult());

	// nuget://{packageId}/{version}{/filePath*}
	internal static Task<ListResourceTemplatesResult> ListResourceTemplatesHandler(RequestContext<ListResourceTemplatesRequestParams> context, CancellationToken token)
		=> Task.FromResult(new ListResourceTemplatesResult
		{
			ResourceTemplates = new List<ResourceTemplate>
			{
				new() {
					Name = "nuget_file_content",
					UriTemplate = "nuget://{packageId}/{version}{/filePath*}",
					Description = "Resource URI for file content within a given nuget package.  Package ID is required,  version can be an explicit version or the literal `latest` to fetch the newest version.  The filePath is the path within the NuGet Package zip archive to return the binary contents of.",
				},
				new() {
					Name = "nuget_package",
					UriTemplate = "nuget://{packageId}/{version}",
					Description = "Resource URI for a NuGet Package file zip.  Package ID is required, version can be an explicit version or the literal `latest` to fetch the newest version.",
					MimeType = "application/zip"
				}
			}
		});

	internal static async Task<ReadResourceResult> ReadResourceHandler(RequestContext<ReadResourceRequestParams> context, CancellationToken token)
	{
		if (string.IsNullOrEmpty(context.Params?.Uri))
			throw new ArgumentException("Uri missing", nameof(context));

		var uri = new Uri(context.Params.Uri);

		if (uri.Scheme != "nuget")
			throw new ArgumentException($"Invalid scheme: {uri.Scheme}", nameof(context));

		var packageId = uri.Host;

		var version = uri.Segments.Length > 1 ? uri.Segments[1].Trim('/') : null;

		var filePath = uri.Segments.Length > 2 ? string.Join("/", uri.Segments.Skip(2).Select(s => s.Trim('/'))).Trim('/') : null;

		if (string.IsNullOrEmpty(packageId))
			throw new ArgumentException("Package ID missing", nameof(context));

		if (string.IsNullOrEmpty(version))
			throw new ArgumentException("Version missing", nameof(context));

		NuGetVersion? packageVersion = null;

		if (version.Equals("latest", StringComparison.OrdinalIgnoreCase))
		{
			packageVersion = await NuGetUtil.GetLatestPackageVersionAsync(packageId, false, null);
		}
		else if (NuGetVersion.TryParse(version, out var parsedVersion))
		{
			packageVersion = parsedVersion;
		}
		
		if (packageVersion is null)
			throw new ArgumentException($"Invalid version: {version}", nameof(context));

		string? blob = null;
		string? mimeType = null;

		if (string.IsNullOrEmpty(filePath))
		{
			var packageData = await NuGetUtil.GetPackageBytesAsync(packageId, packageVersion.ToString(), true, null);
			if (packageData is null)
				throw new ArgumentException($"Package not found: {packageId}", nameof(context));

			mimeType = "application/zip";
			blob = Convert.ToBase64String(packageData);
		}
		else
		{
			var fileData = await NuGetUtil.GetPackageFileAsync(packageId, filePath, packageVersion.ToString(), true, null);
			if (fileData is null)
				throw new ArgumentException($"File not found in {packageId}: {filePath}", nameof(context));
			blob = Convert.ToBase64String(fileData);
		}

		return new ReadResourceResult
		{
			Contents =
			[
				new() {
					Uri = uri.ToString(),
					MimeType = mimeType,
					Blob = blob
				}
			]
		};
	}
}
