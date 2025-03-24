using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace MauiDevEnv;

[McpToolType]
public class AndroidSdkTools
{
    //[McpTool("find_android_sdk")]
    [Description("Returns the best suitable Android SDK path for the current system.")]
    public static string? FindAndroidSdk()
    {
        var m = new AndroidSdk.SdkManager();
        m.SkipVersionCheck = true;
        return m.AndroidSdkHome?.FullName;
    }

   //[McpTool("find_java_jdk")]
    [Description("Returns the best suitable Java JDK path for the current system.")]
    public static string? FindJavaJdk()
    {
        var m = new AndroidSdk.JdkLocator();
        var jdk = m.LocateJdk().FirstOrDefault();
        return jdk?.Home?.FullName;
    }


    //[McpTool("android_sdk_installed_packages")]
    [Description("Returns installed android sdk packages in JSON format.")]
    public static string? InstalledAndroidSdkPackages(
        [Description("Android SDK Home path. If not provided, the default Android SDK path will be used.")]
        string? android_sdk_home = null)
    {
        var m = new AndroidSdk.SdkManager(android_sdk_home);
        m.SkipVersionCheck = true;
        var packages = m.List();
        if (packages == null)
            return null;

        var sb = new StringBuilder();
        sb.AppendLine("{ \"installed_packages\": [");
        foreach (var package in packages.InstalledPackages)
        {
            sb.AppendLine("{ \"id\": \"" + package.Path + "\", \"version\": \"" + package.Version + "\", \"location\": \"" + package.Location + "\", \"description\": \"" + package.Description + "\" },");
        }
        sb.AppendLine("] }");

        return sb.ToString();
    }

    [McpTool("android_sdk_accept_licenses")]
    [Description("Accepts any Android SDK licenses that are not already accepted.  Returns a bool value to indicate if any licenses were accepted.")]
    public static bool AndroidSdkAcceptLicenses(
        [Description("Android SDK Home path. If not provided, the default Android SDK path will be used.")]
        string? android_sdk_home = null)
    {
        var m = new AndroidSdk.SdkManager(android_sdk_home);
        m.SkipVersionCheck = true;

        return m.AcceptLicenses();
    }


    [McpTool("android_sdk_install_package")]
    [Description("Installs the specified Android SDK package.  Returns a bool value to indicate if the installation was successful.")]
    public static bool AndroidSdkInstallPackage(
        [Description("Android SDK package path / ID. (e.g., 'platform-tools' or 'platforms;android-33').")]
        string package_path_or_id,
        [Description("Android SDK Home path. If not provided, the default Android SDK path will be used.")]
        string? android_sdk_home = null)
    {
        var m = new AndroidSdk.SdkManager(android_sdk_home);
        m.SkipVersionCheck = true;

        return m.Install(package_path_or_id);
    }

    [McpTool("android_sdk_uninstall_package")]
    [Description("Uninstalls the specified Android SDK package.  Returns a bool value to indicate if the installation was successful.")]
    public static bool AndroidSdkUninstallPackage(
        [Description("Android SDK package path / ID. (e.g., 'platform-tools' or 'platforms;android-33').")]
        string package_path_or_id,
        [Description("Android SDK Home path. If not provided, the default Android SDK path will be used.")]
        string? android_sdk_home = null)
    {
        var m = new AndroidSdk.SdkManager(android_sdk_home);
        m.SkipVersionCheck = true;

        return m.Uninstall(package_path_or_id);
    }

    [McpTool("android_sdk_download")]
    [Description("Uninstalls the specified Android SDK package.  Returns a bool value to indicate if the installation was successful.")]
    public static async Task<bool> DownloadAndroidSdk(
        [Description("Android SDK Home path to download and extract the SDK into.")]
        string android_sdk_home)
    {
        AndroidSdk.SdkManager m = new AndroidSdk.SdkManager();
        m.SkipVersionCheck = true;
        await m.DownloadSdk(new DirectoryInfo(android_sdk_home));

        return true;
    }

    [McpTool("get_android_environment_info")]
    [Description("Returns combined information about Android SDK path, Java JDK path, and installed Android SDK packages in JSON format.")]
    public static string GetAndroidEnvironmentInfo()
    {
        var androidSdkPath = FindAndroidSdk();
        var javaJdkPath = FindJavaJdk();
        var installedPackages = InstalledAndroidSdkPackages(androidSdkPath);

        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine($"  \"android_sdk_path\": {(androidSdkPath != null ? $"\"{androidSdkPath}\"" : "null")},");
        sb.AppendLine($"  \"java_jdk_path\": {(javaJdkPath != null ? $"\"{javaJdkPath}\"" : "null")},");
        
        if (installedPackages != null)
        {
            // Remove the outer brackets from the packages JSON since we're incorporating it
            var packagesJson = installedPackages.Trim()
                .TrimStart('{')
                .TrimEnd('}')
                .Trim();
            sb.AppendLine($"  {packagesJson}");
        }
        else
        {
            sb.AppendLine("  \"installed_packages\": []");
        }
        
        sb.AppendLine("}");

        return sb.ToString();
    }
}