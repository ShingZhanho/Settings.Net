using System.Diagnostics.CodeAnalysis;

namespace Settings.Net
{
    /// <summary> All constant values of Settings.Net. </summary>
    [ExcludeFromCodeCoverage]
    internal static class Constants
    {
        /// <summary> Flags indicating the entry's type. </summary>
        internal static class EntryTypeFlags
        {
            internal static string StringEntry => "Settings.StringEntry";
            internal static string IntEntry => "Settings.IntEntry";
            internal static string BoolEntry => "Settings.BoolEntry";
            internal static string EntryGroup => "Settings.EntryGroup";
        }
    }
}