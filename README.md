# .NET MCP Servers
Collection of my MCP (Model Context Protocol) servers written in .NET

-----------------------------------


## NuGet
NuGet MCP Server

### Tools

#### Search
Searches NuGet with a given query
- Name: `nuget_search`
- Parameters: 
    - `query` (Uses nuget query syntax)
    - `nuget_sources` (optional, default is NuGet.org)
    - `allow_prerelease` (default: false)
    - `skip` (default: 0)
    - `take` (default: 30)

### Resource Templates

#### NuGet Package File Content
Specify a NuGet Package ID, Version (or `latest` for the newest), and a File Path of a file within the NuGet package to retrieve
 - Name: `nuget_file_content`
 - URI Template: `nuget://{packageId}/{version}{/filePath*}`

#### NuGet Package Content
Specify a NuGet Package ID, Version (or `latest` for the newest) to retrieve the NuGet package zip content of
 - Name: `nuget_package`
 - URI Template: `nuget://{packageId}/{version}`



