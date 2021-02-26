#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Settings.Net.Exceptions;

namespace Settings.Net {
    /// <summary>Represents an entry.</summary>
    public class SettingEntry<T> {
        public SettingEntry(string id, T? value, string description) {
            // Check for accepted types
            if (!new[] {typeof(string), typeof(int)}.Contains(typeof(T)))
                throw new ArgumentOutOfRangeException(nameof(T), "This type is not accepted." +
                                                                 " Accepts only string, int and bool");
            // Check for illegal chars in ID
            if (!IdIsValid(id))
                throw new InvalidNameException(nameof(id), "IDs should not contain chars: " +
                                                           $"'{GetInvalidIdCharsInString(id)}'");
            // Assign TypeOfEntry
            var enumDict = new Dictionary<Type, EntryType>();
            enumDict.Add(typeof(string), EntryType.StringType);
            enumDict.Add(typeof(int), EntryType.IntType);
            enumDict.Add(typeof(bool), EntryType.BoolType);
            TypeOfEntry = enumDict[typeof(T)];
            // Assign properties
            ID = id;
            Description = description;
            Value = value;
        }
        
        /// <summary> The ID of the entry. The ID is unique under an entry group. </summary>
        public string ID { get; }

        /// <summary> A descriptive line of this entry. Description is optional. </summary>
        public string Description { get; set; }
        
        /// <summary>The value of this entry.</summary>
        public T? Value { get; internal set; }
        
        /// <summary>Gets the type of this entry.</summary>
        public EntryType TypeOfEntry { get; }

        /// <summary>Gets an array of invalid ID characters.</summary>
        public static char[] InvalidIdChars { get; } = {'.', ' '};

        /// <summary> Checks if the given ID is a valid ID.</summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>true if valid, false instead.</returns>
        internal static bool IdIsValid(string id) => id.IndexOfAny(InvalidIdChars) == -1;

        /// <summary> Gets the list of chars that are illegal to use for an ID name. </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdCharsInString(string str) => str.Where(character => InvalidIdChars.Contains(character)).ToArray();

        public enum EntryType {
            StringType,
            IntType,
            BoolType
        }
    }
}