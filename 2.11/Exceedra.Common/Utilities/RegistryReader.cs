using Microsoft.Win32;

namespace Exceedra.Common.Utilities
{
    public static class RegistryReader
    {
        public static string GetOsVersion()
        {
            RegistryKey windowsCurrentVersionKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (windowsCurrentVersionKey == null) return string.Empty;

            var osVersionFull = string.Empty;

            var windowsName = windowsCurrentVersionKey.GetValue("ProductName");
            if (windowsName != null) osVersionFull += windowsName;

            var csdVersion = windowsCurrentVersionKey.GetValue("CSDVersion");
            if (csdVersion != null) osVersionFull += " " + csdVersion;

            var currentVersion = windowsCurrentVersionKey.GetValue("CurrentVersion");
            var currentBuild = windowsCurrentVersionKey.GetValue("CurrentBuild");

            if (currentVersion != null && currentBuild != null)
                osVersionFull +=  " (v" + currentVersion + "." + currentBuild + ")";

            return osVersionFull;
        }

        public static string GetNetFrameworkVersion()
        {
            RegistryKey net40FrameworkKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
            if (net40FrameworkKey == null) return ".Net Framework not installed or below 4.0";

            var netFrameworkVersion = net40FrameworkKey.GetValue("Version").ToString();

            return netFrameworkVersion;
        }

        public static string GetIeVersion()
        {
            var internetExplorerKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer");
            if (internetExplorerKey == null) return string.Empty;

            var internetExplorerVersion = internetExplorerKey.GetValue("Version");
            if (internetExplorerVersion == null) return string.Empty;

            return internetExplorerVersion.ToString();
        }
    }
}
