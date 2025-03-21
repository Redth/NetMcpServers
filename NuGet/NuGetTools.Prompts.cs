namespace NetMcp.NuGet;

using System.Text.Json;

partial class NuGetTools
{
    /// <summary>
    /// Encodes a string to make it safe for use in a JSON value.
    /// Uses System.Text.Json to properly handle JSON string escaping.
    /// </summary>
    /// <param name="input">The input string to encode</param>
    /// <returns>A JSON-safe encoded version of the input string</returns>
    internal static string JsonSafeEncode(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
            
        // Use JsonSerializer to properly escape the string
        var jsonString = JsonSerializer.Serialize(input);
        
        // Remove the surrounding quotes that JsonSerializer adds
        return jsonString.Substring(1, jsonString.Length - 2);
    }

    internal const string NuGetSearchDescription = @"NuGet Search can be used to search for packages in the given NuGet feed(s).
## Notes:
- Package IDs are case-insensitive
- URLs use lowercase package IDs and versions
- Version strings must be normalized according to SemVer rules

## Response Formats
- Most API responses are JSON
- Pagination is typically handled via skip/take parameters
- HTTP status codes follow standard conventions (200 success, 404 not found, etc.)";

    internal const string NuGetSearchQueryDescription = @"The search query to use.

# Search Syntax:

## Basic Search
- Simple terms: `json` (matches packages with ""json"" in ID, title, description, tags)
- Multiple terms: `json parser` (matches packages containing both terms)
- Phrases: `""json parser""` (matches exact phrase)

## Field-Specific Queries
- ID filter: `id:newtonsoft.json` (exact ID match)
- Package title: `title:json` (matches in title)
- Tags: `tags:logging` (matches packages with specific tag)
- Author: `author:Microsoft` (filters by author)

## Logical Operators
- AND operator: `json AND parser` (both terms required)
- OR operator: `json OR xml` (either term)
- NOT operator: `json NOT newtonsoft` (excludes matches)
- Grouping: `(json OR xml) AND parser` (combines operators)

## Wildcard Support
- Trailing wildcard: `new*` (matches terms starting with ""new"")
- Note: Leading wildcards (`*soft`) are not supported on nuget.org

## Version Filtering
- Version range: `packageid:EntityFramework version:6.2.0` (specific version)
- Version range: `packageid:EntityFramework version:(>=5.0 <6.0)` (range)

## Special Characters
- Special characters need to be URL-encoded
- Spaces can be represented as `+` or `%20` in URLs

## Examples
- `id:NLog+tags:logging` (packages with ID ""NLog"" and tag ""logging"")
- `SignalR+author:Microsoft` (Microsoft SignalR packages)
- `json+NOT+newtonsoft` (json packages excluding Newtonsoft)

Note: Exact query syntax support may vary between different NuGet server implementations. This reference primarily covers nuget.org's implementation.
";

}