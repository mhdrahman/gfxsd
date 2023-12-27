using System;
using System.IO;

namespace GFXSD
{
    // TODO need to be able to pass the tool paths in via app settings
    public static class Configuration
    {
        public static string DataDirectory;

        public static string Terminal;

        public static string Xsd2InstToolPath;

        public static string XsdToolPath;

        static Configuration()
        {
            var isLinux = Environment.OSVersion.Platform == PlatformID.Unix;

            DataDirectory = isLinux
                ? Path.Combine(Environment.GetEnvironmentVariable("HOME"), "opt", "GFXSD")
                : @"C:\ProgramData\GFXSD\Data";

            Terminal = isLinux 
                ? "/bin/bash"
                : "cmd.exe";

            // The /C at the beginning of the windows path is so the process closes after the command finishes running
            Xsd2InstToolPath = isLinux
                ? "/home/xmlbeans/xmlbeans-5.2.0/bin/xsd2inst"
                : @"/C C:\ProgramData\GFXSD\xmlbeans-5.2.0\bin\xsd2inst.cmd";

            XsdToolPath = @"External\xsd.exe";

            Directory.CreateDirectory(DataDirectory);
        }
    }
}
