# MauiDevEnv MCP Server - Agent Documentation

## Overview
This Model Context Protocol (MCP) server provides tools for managing .NET MAUI development environments. It helps with .NET SDK/runtime discovery, Android SDK management, Java JDK location, and workload configuration.

**Package**: `Mcp.Server.MauiDevEnv`  
**Tool Command**: `mcp-server-mauidevenv`  
**Target Frameworks**: net8.0, net9.0, net10.0

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

Add to your MCP client's configuration file (e.g., Claude Desktop):

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

## MCP Tools

### .NET Tools

#### `dotnet_info`
Retrieves comprehensive information about installed .NET SDKs, runtimes, and workloads.

**Parameters**: None

**Returns**: JSON object containing:
- SDK information (version, commit, workload version, MSBuild version)
- Host information (version, architecture, commit)
- Runtime environment (OS details, RID, base path)
- Installed workloads (name, version, manifest path, install type, dependencies)
- List of installed SDKs with paths
- List of installed runtimes with paths
- Other architectures (if any)
- Environment variables
- Global.json path (if present)
- Latest available SDK version from NuGet releases

**Implementation**: `DotnetTools.cs:GetDotNetInfo()`

**Example Output Structure**:
```json
{
  "sdk": {
    "version": "9.0.101",
    "commit": "abc123",
    "workloadVersion": "9.0.100-manifests.12345",
    "msBuildVersion": "17.12.0"
  },
  "host": {
    "version": "9.0.1",
    "architecture": "x64",
    "commit": "def456"
  },
  "installedSdks": [
    {"version": "8.0.404", "path": "/usr/local/share/dotnet/sdk/8.0.404"},
    {"version": "9.0.101", "path": "/usr/local/share/dotnet/sdk/9.0.101"}
  ],
  "workloads": [
    {
      "name": "maui-ios",
      "manifestVersion": "9.0.10/9.0.100",
      "manifestPath": "/path/to/manifest",
      "installType": "FileBased"
    }
  ],
  "latestAvailableSdkVersion": "9.0.101"
}
```

### Android SDK Tools

#### `get_android_environment_info`
Returns combined information about Android SDK, Java JDK, and installed Android SDK packages.

**Parameters**: None

**Returns**: JSON object with:
- `android_sdk_path` - Path to Android SDK or null if not found
- `java_jdk_path` - Path to Java JDK or null if not found
- `installed_packages` - Array of installed Android SDK packages with details

**Implementation**: `AndroidSdkTools.cs:GetAndroidEnvironmentInfo()`

#### `android_sdk_accept_licenses`
Accepts any Android SDK licenses that are not already accepted.

**Parameters**:
- `android_sdk_home` (string, optional) - Android SDK path. If not provided, uses default location.

**Returns**: Boolean indicating if any licenses were accepted

**Implementation**: `AndroidSdkTools.cs:AndroidSdkAcceptLicenses()`

#### `android_sdk_install_package`
Installs a specified Android SDK package.

**Parameters**:
- `package_path_or_id` (string, required) - Package identifier (e.g., "platform-tools", "platforms;android-34")
- `android_sdk_home` (string, optional) - Android SDK path

**Returns**: Boolean indicating installation success

**Implementation**: `AndroidSdkTools.cs:AndroidSdkInstallPackage()`

**Common Package IDs**:
- `platform-tools` - ADB and other platform tools
- `platforms;android-34` - Android 14 SDK platform
- `build-tools;34.0.0` - Build tools for Android 14
- `emulator` - Android Emulator
- `system-images;android-34;google_apis;x86_64` - System image for emulator

#### `android_sdk_uninstall_package`
Uninstalls a specified Android SDK package.

**Parameters**:
- `package_path_or_id` (string, required) - Package identifier
- `android_sdk_home` (string, optional) - Android SDK path

**Returns**: Boolean indicating uninstallation success

**Implementation**: `AndroidSdkTools.cs:AndroidSdkUninstallPackage()`

#### `android_sdk_download`
Downloads and extracts the Android SDK to a specified location.

**Parameters**:
- `android_sdk_home` (string, required) - Target path to download and extract SDK

**Returns**: Boolean indicating download success

**Implementation**: `AndroidSdkTools.cs:DownloadAndroidSdk()`

### Helper Functions (Not Exposed as Tools)

#### `FindAndroidSdk()`
Locates the best suitable Android SDK path for the current system.
Uses the `AndroidSdk.SdkManager` library to find default locations.

#### `FindJavaJdk()`
Locates the best suitable Java JDK path for the current system.
Uses the `AndroidSdk.JdkLocator` library.

#### `InstalledAndroidSdkPackages()`
Lists all installed Android SDK packages with details.

## Code Structure

### Core Files

#### `Program.cs`
Entry point that:
- Calls `GetDotNetInfo()` on startup
- Configures MCP server with stdio transport
- Registers tools from assembly
- Disables logging

#### `DotnetTools.cs`
Comprehensive .NET environment inspection:
- `GetDotNetInfo()` - Main tool implementation
- `ParseDotnetInfoOutput()` - Parses `dotnet --info` command output
- `GetDotNetReleasesInfo()` - Fetches latest SDK versions from Microsoft's releases API
- Section processors for each part of dotnet info output:
  - `ProcessSdkSection()`
  - `ProcessHostSection()`
  - `ProcessRuntimeEnvironmentSection()`
  - `ProcessWorkloadsSection()` - Includes reading WorkloadDependencies.json
  - `ProcessSdksSection()`
  - `ProcessRuntimesSection()`
  - `ProcessOtherArchitecturesSection()`
  - `ProcessEnvironmentVariablesSection()`
  - `ProcessGlobalJsonSection()`

#### `AndroidSdkTools.cs`
Android SDK management using the `AndroidSdk` NuGet package:
- SDK location detection
- JDK location detection
- License acceptance
- Package installation/uninstallation
- SDK download functionality

#### `DotnetModels/`
Data models for strongly-typed .NET information:
- `DotnetInfo` - Root model
- `SdkInfo` - SDK details
- `HostInfo` - Host details
- `RuntimeEnvironmentInfo` - Runtime/OS details
- `WorkloadInfo` - Workload configuration with dependencies
- `SdkVersionInfo` - SDK version and path
- `RuntimeInfo` - Runtime name, version, path
- `ArchitectureInfo` - Alternative architecture installations

## Technical Details

### .NET Info Parsing
The tool executes `dotnet --info` and parses its output:
- Text-based parsing with section detection
- Handles variable section ordering
- Regex patterns for extracting structured data
- Graceful handling of missing sections

### Workload Dependencies
For each installed workload, the tool:
1. Locates the manifest path
2. Finds `WorkloadDependencies.json` in the manifest directory
3. Parses JSON to include dependency information
4. Stores as JsonElement for flexible access

### Android SDK Integration
Uses the `AndroidSdk` NuGet package (https://github.com/Redth/AndroidSdk.Tools):
- Automatic SDK location detection (ANDROID_HOME, default paths)
- Cross-platform support (Windows, macOS, Linux)
- Direct integration with Android SDK Manager
- License file management

### Java JDK Detection
JdkLocator searches common locations:
- JAVA_HOME environment variable
- System default paths
- macOS: `/Library/Java/JavaVirtualMachines/`
- Windows: `C:\Program Files\Java\`
- Linux: `/usr/lib/jvm/`

## Dependencies
- `Microsoft.Deployment.DotNet.Releases` - Official .NET releases metadata
- `AndroidSdk` (via `AndroidSdk.SdkManager` and `AndroidSdk.JdkLocator`) - Android SDK management
- `ModelContextProtocol` - MCP server framework
- `System.Text.Json` - JSON serialization

## Usage Scenarios

### For LLM Agents Setting Up MAUI Development

1. **Initial Environment Check**:
   ```
   Call dotnet_info to see what's installed
   Call get_android_environment_info to check Android setup
   ```

2. **Install Missing Components**:
   ```
   If Android SDK missing: Call android_sdk_download
   If licenses not accepted: Call android_sdk_accept_licenses
   If platform missing: Call android_sdk_install_package with "platforms;android-34"
   ```

3. **Verify Installation**:
   ```
   Call get_android_environment_info again to confirm packages installed
   ```

### For Development Tools

1. **Environment Diagnostics**: Quick check of SDK/runtime versions and workloads
2. **CI/CD Setup**: Automated environment configuration
3. **Dependency Tracking**: Monitor installed SDKs and runtimes
4. **Workload Management**: Verify required workloads are installed
5. **License Compliance**: Ensure Android SDK licenses are accepted

## Common Workflows

### Setting up a new .NET MAUI development environment:
1. Check existing setup with `dotnet_info`
2. Check Android environment with `get_android_environment_info`
3. Download Android SDK if missing with `android_sdk_download`
4. Accept licenses with `android_sdk_accept_licenses`
5. Install required platform with `android_sdk_install_package`
6. Verify with `get_android_environment_info`

### Troubleshooting build issues:
1. Run `dotnet_info` to check SDK version and workloads
2. Run `get_android_environment_info` to verify Android SDK packages
3. Compare against project requirements
4. Install missing components as needed
