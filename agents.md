# NetMcpServers - Repository Overview for LLMs

## Repository Purpose
This repository contains .NET implementations of Model Context Protocol (MCP) servers. MCP is a protocol that enables LLMs to interact with external tools and resources through a standardized interface. These servers are published as NuGet packages and can be run as dotnet tools.

## Architecture
- **Language**: C# / .NET
- **Framework**: Uses the `ModelContextProtocol` NuGet package (v0.5.0-preview.1, latest as of Dec 2025)
- **Transport**: Stdio-based server transport
- **Structure**: Multi-project solution with individual MCP servers
- **Packaging**: Published as NuGet packages with `PackAsTool=true`
- **Target Frameworks**: net8.0, net9.0, net10.0

## Installation & Usage

### Running with dnx (dotnet 10.0+)
The `dnx` command in .NET 10 allows running dotnet tools directly from NuGet without installation:
```bash
dnx Mcp.Server.MauiDevEnv --yes
```

### Running with dotnet tool (dotnet 8.0+)
For .NET 8 or 9, install as global tools:
```bash
dotnet tool install --global Mcp.Server.MauiDevEnv
mcp-server-mauidevenv
```

## MCP Client Configuration
Each project includes an `mcp.json` configuration file that can be used with MCP clients (Claude Desktop, GitHub Copilot, etc.):

```json
{
  "mcpServers": {
    "mauidevenv": {
      "type": "stdio",
      "command": "dnx",
      "args": ["Mcp.Server.MauiDevEnv", "--yes"]
    }
  }
}
```

## Projects

### MauiDevEnv MCP Server (`/MauiDevEnv`)
A Model Context Protocol server focused on .NET MAUI development environment setup and configuration.

**Key Files**:
- `Program.cs` - Entry point, configures MCP server
- `DotnetTools.cs` - Tools for .NET SDK/runtime information
- `AndroidSdkTools.cs` - Tools for Android SDK management
- `DotnetModels/` - Data models for .NET information

**Capabilities**:
- Query .NET SDK and runtime information
- Manage Android SDK packages
- Accept Android SDK licenses
- Locate Java JDK installations

## Common Patterns

### MCP Tool Definition
Tools are defined using attributes:
```csharp
[McpServerToolType]
public class ToolClass
{
    [McpServerTool(Name = "tool_name")]
    [Description("Tool description")]
    public static async Task<string> ToolMethod(
        [Description("Parameter description")]
        string parameter)
    {
        // Implementation
    }
}
```

### MCP Resource Templates
Resources provide URI-based access to data:
```csharp
internal static ValueTask<ListResourceTemplatesResult> ListResourceTemplatesHandler(...)
{
    return new ListResourceTemplatesResult
    {
        ResourceTemplates = new List<ResourceTemplate>
        {
            new() {
                Name = "resource_name",
                UriTemplate = "scheme://{param1}/{param2}",
                Description = "Resource description"
            }
        }
    };
}
```

### Server Configuration
The server uses the following setup pattern:
```csharp
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithListResourceTemplatesHandler(...)
    .WithReadResourceHandler(...);
```

## Building & Running
- Solution file: `NetMcp.sln`
- Standard .NET CLI commands apply:
  - `dotnet build` - Build the solution
  - `dotnet run --project MauiDevEnv` - Run MauiDevEnv server

## Configuration Files
- `Directory.Build.props` - Common MSBuild properties
- `Directory.packages.props` - Central package management
- `version.json` - Version configuration (per project)

## Dependencies
The project depends on:
- `ModelContextProtocol` - Core MCP implementation
- Various .NET libraries specific to the domain (Microsoft.Deployment.DotNet.Releases, AndroidSdk, NuGet.Protocol, etc.)

## Deployment
Output artifacts are placed in the `/artifacts` directory.
