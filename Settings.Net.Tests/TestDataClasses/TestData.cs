using System;
using System.IO;

namespace Settings.Net.Tests
{
    public static partial class TestData
    {
        private static string CurrentDir => Environment.CurrentDirectory;
        private static string FolderPath => Path.Combine(CurrentDir, "TestData");
    }
}