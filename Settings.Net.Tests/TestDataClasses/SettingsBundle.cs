using System.IO;

namespace Settings.Net.Tests
{
    public static partial class TestData
    {
        public static class SettingsBundle
        {
            private static string SettingsBundleDataFolder => Path.Combine(FolderPath, "SettingsBundle");

            public static string NormalJsonFilePath => Path.Combine(SettingsBundleDataFolder, "NormalSettingsFile.sbd");
            public static string InvalidJsonFilePath => Path.Combine(SettingsBundleDataFolder, "Error_InvalidJson.sbd");
            public static string JsonWithoutMetadataFilePath => Path.Combine(SettingsBundleDataFolder, "Error_WithoutMetadata.sbd");
            public static string JsonWithoutDataFilePath => Path.Combine(SettingsBundleDataFolder, "Error_WithoutData.sbd");
        }
    }
}