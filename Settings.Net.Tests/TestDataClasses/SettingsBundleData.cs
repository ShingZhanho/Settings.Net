using System;
using System.IO;

namespace Settings.Net.Tests
{
    public static class TestData
    {
        private static string CurrentDir => Environment.CurrentDirectory;
        private static string FolderPath => Path.Combine(CurrentDir, "TestData");
        
        public static class SettingsBundleData
        {
            private static string SettingsBundleDataFolder =>
                Path.Combine(FolderPath, "SettingsBundle");

            public static string NormalJsonFilePath => Path.Combine(SettingsBundleDataFolder, "NormalSettingsFile.sbd");
            public static string InvalidJsonFilePath => Path.Combine(SettingsBundleDataFolder, "Error_InvalidJson.sbd");
            public static string UnreadableFilePath => Path.Combine(SettingsBundleDataFolder, "Error_Unreadable.sbd");
        }
    }
}