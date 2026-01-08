# .NET MCP Servers
Collection of MCP (Model Context Protocol) servers written in .NET, published as NuGet packages.

## Installation

### Using dnx (dotnet 10.0+)
With .NET 10 or later, you can run the servers directly without installation:

```bash
dnx Mcp.Server.MauiDevEnv --yes
```

### Using dotnet tool (dotnet 8.0+)
For .NET 8 or 9, install as global tools:

```bash
dotnet tool install --global Mcp.Server.MauiDevEnv
```

## Configuration

### MCP Client Configuration
Add to your MCP client configuration file (e.g., Claude Desktop, GitHub Copilot):

**For .NET 10+** (using dnx):
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

**For .NET 8/9** (using installed tools):
```json
{
  "mcpServers": {
    "mauidevenv": {
      "type": "stdio",
      "command": "mcp-server-mauidevenv"
    }
  }
}
```

See individual `mcp.json` files in each project folder for ready-to-use configurations.

---

## Servers

### MauiDevEnv MCP Server
Package: `Mcp.Server.MauiDevEnv`  
Command: `mcp-server-mauidevenv` or `dnx Mcp.Server.MauiDevEnv`

Manage .NET MAUI development environments including .NET SDKs, Android SDK, and Java JDK.

#### Tools

**`dotnet_info`** - Get comprehensive .NET SDK and runtime information
- Returns: JSON with SDK versions, workloads, runtimes, and latest available versions

**`get_android_environment_info`** - Get Android SDK and Java JDK information
- Returns: JSON with Android SDK path, Java JDK path, and installed packages

**`android_sdk_accept_licenses`** - Accept Android SDK licenses
- Parameters: `android_sdk_home` (optional)

**`android_sdk_install_package`** - Install Android SDK packages
- Parameters: `package_path_or_id`, `android_sdk_home` (optional)

**`android_sdk_uninstall_package`** - Uninstall Android SDK packages
- Parameters: `package_path_or_id`, `android_sdk_home` (optional)

**`android_sdk_download`** - Download Android SDK
- Parameters: `android_sdk_home` (required)

---

## Development

### Testing Locally
```bash
# Test MauiDevEnv server
dotnet run --project MauiDevEnv
```

## Documentation
- See `agents.md` files in each project for detailed LLM-friendly documentation
- Root `agents.md` provides repository overview



