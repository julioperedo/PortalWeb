using System;
using System.IO;
using System.Runtime.InteropServices;

internal static class IISHelper
{
    public static string GetContentRoot()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && NativeMethods.IsAspNetCoreModuleLoaded())
        {
            var iisConfigData = NativeMethods.HttpGetApplicationProperties();
            var contentRoot = iisConfigData.pwzFullApplicationPath.TrimEnd(Path.DirectorySeparatorChar);
            return contentRoot;
        }
        return null;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IISConfigurationData
    {
        public IntPtr pNativeApplication;
        [MarshalAs(UnmanagedType.BStr)]
        public string pwzFullApplicationPath;
        [MarshalAs(UnmanagedType.BStr)]
        public string pwzVirtualApplicationPath;
        public bool fWindowsAuthEnabled;
        public bool fBasicAuthEnabled;
        public bool fAnonymousAuthEnable;
    }

    private static class NativeMethods
    {
        public static bool IsAspNetCoreModuleLoaded()
        {
            return GetModuleHandle("aspnetcorev2_inprocess.dll") != IntPtr.Zero;
        }

        public static IISConfigurationData HttpGetApplicationProperties()
        {
            var iisConfigurationData = default(IISConfigurationData);
            var errorCode = http_get_application_properties(ref iisConfigurationData);
            if (errorCode != 0) throw Marshal.GetExceptionForHR(errorCode);
            return iisConfigurationData;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("aspnetcorev2_inprocess.dll")]
        private static extern int http_get_application_properties(ref IISConfigurationData iiConfigData);
    }
}
