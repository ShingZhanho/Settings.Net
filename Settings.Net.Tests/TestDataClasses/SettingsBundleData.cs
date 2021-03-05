using System;
using System.IO;

namespace Settings.Net.Tests
{
    public partial class TestData
    {
        private static string CurrentDir => Environment.CurrentDirectory;
        private static string FolderPath => Path.Combine(CurrentDir, "TestData");
        
        public static class SettingsBundleData
        {
            private static string FolderPath =>
                Path.Combine(TestData.FolderPath, "SettingsBundle");

            public static string NormalJsonFilePath => Path.Combine(FolderPath, "NormalSettingsFile.sj");
        }
    }
}