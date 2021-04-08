using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Settings.Net.Tests
{
    public static partial class TestData
    {
        public static class SettingEntry
        {
            private static string SettingEntryDataFolder => Path.Combine(FolderPath, "SettingEntry");

            // For testing invalid IDs
            public static string StringEntryWithInvalidIdJsonPath => Path.Combine(SettingEntryDataFolder, "Error_StringEntryWithInvalidId.json");
            public static string IntEntryWithInvalidIdJsonPath => Path.Combine(SettingEntryDataFolder, "Error_IntEntryWithInvalidId.json");
            public static string BoolEntryWithInvalidJsonPath => Path.Combine(SettingEntryDataFolder, "Error_BoolEntryWithInvalidId.json");

            public static string StringEntryWithoutTypeKeyJsonPath => Path.Combine(SettingEntryDataFolder, "Error_StringEntryWithoutTypeKey.json");
        }
    }
}