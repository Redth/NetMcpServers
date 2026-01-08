# MauiDevEnv MCP Server

A Model Context Protocol (MCP) server for managing .NET MAUI development environments. This server helps with .NET SDK/runtime discovery, Android SDK management, Java JDK location, and workload configuration.

## Installation

### Using dnx (.NET 10+)
```bash
dnx Mcp.Server.MauiDevEnv --yes
```

### Using dotnet tool (.NET 8+)
```bash
dotnet tool install --global Mcp.Server.MauiDevEnv
mcp-server-mauidevenv
```

## MCP Client Configuration

Add to your MCP client's configuration file (e.g., Claude Desktop, GitHub Copilot):

**For .NET 10+**:
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

**For .NET 8/9**:
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

## Tools

### `dotnet_info`
Gets comprehensive information about installed .NET SDKs, runtimes, workloads, and the latest available SDK version.

**Returns**: JSON with SDK versions, workloads, runtimes, and latest available versions.

### `get_android_environment_info`
Returns combined information about Android SDK path, Java JDK path, and installed Android SDK packages.

**Returns**: JSON with Android SDK path, Java JDK path, and installed packages.

### `android_sdk_accept_licenses`
Accepts any Android SDK licenses that are not already accepted.

**Parameters**:
- `android_sdk_home` (optional): Android SDK home path. Uses default if not provided.

**Returns**: Boolean indicating if any licenses were accepted.

### `android_sdk_install_package`
Installs the specified Android SDK package.

**Parameters**:
- `package_path_or_id` (required): Package path/ID (e.g., `platform-tools` or `platforms;android-33`)
- `android_sdk_home` (optional): Android SDK home path. Uses default if not provided.

**Returns**: Boolean indicating if installation was successful.

### `android_sdk_uninstall_package`
Uninstalls the specified Android SDK package.

**Parameters**:
- `package_path_or_id` (required): Package path/ID (e.g., `platform-tools` or `platforms;android-33`)
- `android_sdk_home` (optional): Android SDK home path. Uses default if not provided.

**Returns**: Boolean indicating if uninstallation was successful.

### `android_sdk_download`
Downloads and extracts the Android SDK to the specified location.

**Parameters**:
- `android_sdk_home` (required): Path to download and extract the SDK into.

**Returns**: Boolean indicating if download was successful.

## License

MIT
