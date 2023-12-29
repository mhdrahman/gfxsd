using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace GFXSD
{
    public static class Configuration
    {
        public static string Username;
        public static string Password;
        public static string DataDirectory;
        public static string Terminal;
        public static string Xsd2InstCommand;
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

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var authConfig = config.GetSection("Authentication");
            Username = authConfig.GetValue<string>("Username");
            Password = authConfig.GetValue<string>("Password");

            var toolConfig = config.GetSection("Tools");

            var xsd2Inst = toolConfig.GetSection("Xsd2Inst");
            Xsd2InstCommand = isLinux
                ? xsd2Inst.GetValue<string>("Linux")
                : xsd2Inst.GetValue<string>("Windows"); 

            var xsd = toolConfig.GetSection("Xsd");
            XsdToolPath = isLinux
                ? xsd.GetValue<string>("Linux")
                : xsd.GetValue<string>("Windows");

            Directory.CreateDirectory(DataDirectory);
        }
    }
}
