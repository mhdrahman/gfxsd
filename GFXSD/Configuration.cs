using System;
using System.IO;

namespace GFXSD
{
    public static class Configuration
    {
        public static bool IsLinux;

        public static string DataDirectory;

        public static string Xsd2InstToolPath;

        public static string XsdToolPath;

        static Configuration()
        {
            IsLinux = Environment.OSVersion.Platform == PlatformID.Unix;

            DataDirectory = IsLinux
                ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "opt", "GFXSD")
                : "C:/ProgramData/GFXSD";

            Xsd2InstToolPath = IsLinux
                ? "/home/xmlbeans/xmlbeans-5.2.0/bin/xsd2inst"
                : @"C:\ProgramData\GFXSD\xmlbeans-5.2.0\bin\xsd2inst.cmd";

            XsdToolPath = @"External\xsd.exe";

            Directory.CreateDirectory(DataDirectory);
        }
    }
}
