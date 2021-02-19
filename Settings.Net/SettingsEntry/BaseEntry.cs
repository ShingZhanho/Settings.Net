using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Settings.Net.SettingsEntry {
    /// <summary> A base class for settings entry. This class is abstract and shall not be used in your code. </summary>
    [ExcludeFromCodeCoverage]
    public abstract class BaseEntry {
        /// <summary> The ID of the entry. The ID is unique under an entry group. </summary>
        public abstract string ID { get; }

        /// <summary> A descriptive line of this entry. Description is optional. </summary>
        public abstract string Description { get; set; }

        /// <summary>Using these chars for ID names is illegal and exception will be thrown.</summary>
        internal static readonly char[] InvalidIdChars = {'.', ' '};

        /// <summary> Checks if the given ID is a valid ID.</summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>true if valid, false instead.</returns>
        internal static bool IdIsValid(string id) => id.IndexOfAny(InvalidIdChars) == -1;

        /// <summary> Gets the list of chars that are illegal to use for an ID name. </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdCharsInString(string str) => str.Where(character => InvalidIdChars.Contains(character)).ToArray();
    }
}