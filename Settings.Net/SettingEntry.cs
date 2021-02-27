#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Exceptions;
using Settings.Net.Exceptions;
#pragma warning disable 8714

namespace Settings.Net {
    /// <summary>Represents an entry with value.</summary>
    public class SettingEntry<TValue> : IEntryNode<TValue> {
        public SettingEntry(string id, TValue? value) {
            // Check for accepted types
            if (!new[] {typeof(string), typeof(int), typeof(bool)}.Contains(typeof(TValue)))
                throw new ArgumentOutOfRangeException(nameof(TValue), "This type is not accepted." +
                                                                 " Accepts only string, int and bool");
            // Check for illegal chars in ID
            if (!IdIsValid(id))
                throw new InvalidNameException(nameof(id), "IDs should not contain chars: " +
                                                           $"'{GetInvalidIdCharsInString(id)}'");
            // Assign Type property
            var enumDict = new Dictionary<Type, EntryType>();
            enumDict.Add(typeof(string), EntryType.String);
            enumDict.Add(typeof(int), EntryType.Int);
            enumDict.Add(typeof(bool), EntryType.Bool);
            Type = enumDict[typeof(TValue)];
            // Assign properties
            Id = id;
            Value = value;
        }

        public SettingEntry(JToken jToken) {
            
        }
        
        public string Id { get; }
        public string? Description { get; set; }
        public TValue? Value { get; }
        public EntryType Type { get; }
        public SettingsGroup? Parent { get; }

        /// <summary>Gets an array of invalid ID characters.</summary>
        public static char[] InvalidIdChars {
            get { return new[] {'.', ' '}; }
        }

        /// <summary> Checks if the given ID is a valid ID.</summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>true if valid, false instead.</returns>
        internal static bool IdIsValid(string id) => id.IndexOfAny(InvalidIdChars) == -1;

        /// <summary> Gets the list of chars that are illegal to use for an ID name. </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdCharsInString(string str) => str.Where(character => InvalidIdChars.Contains(character)).ToArray();

        private static void InternalEnsureJsonState(JToken jToken) {
            // Check ID
            string id;
            try {
                id = ((JObject) jToken).Properties().ToList()[0].Name;
                if (!IdIsValid(id))
                    throw new InvalidNameException(id,
                        $"The ID '{id}' is invalid. These characters are illegal: '{GetInvalidIdCharsInString(id)}'");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException(nameof(Id), "The ID key could not be found in the JSON.");
            }
            // Check entry type
            var typeDictionary = new Dictionary<Type, EntryType> {
                {typeof(string), EntryType.String}, {typeof(int), EntryType.Int}, {typeof(bool), EntryType.Bool}
            };
            try {
                if (jToken[id]!["type"]!.ToString() != typeDictionary[typeof(TValue)].ToString()) {
                    var enumParseSuccess =
                        Enum.TryParse(typeof(EntryType), jToken[id]!["type"]!.ToString(), out var result);
                    throw enumParseSuccess
                        ? new EntryTypeNotMatchException(typeDictionary[typeof(TValue)].ToString(), result!.ToString(),
                            $"Passed in JToken is not a {typeDictionary[typeof(TValue)].ToString()} entry.")
                        : new ArgumentOutOfRangeException(nameof(jToken), $"Unknown type: {jToken[id]!["type"]!}");
                }
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException(nameof(Type), "The Type key could not be found.");
            }
            // Check entry value
            var tokenTypeDictionary = new Dictionary<Type, JTokenType> {
                {typeof(string), JTokenType.String}, {typeof(int), JTokenType.Integer},
                {typeof(bool), JTokenType.Boolean}
            };
            try {
                if (jToken[id]!["contents"]!.Type != tokenTypeDictionary[typeof(TValue)] &&
                    jToken[id]!["contents"]!.Type != JTokenType.Null) {
                    throw new InvalidEntryValueException(
                        $"{tokenTypeDictionary[typeof(TValue)]}' or '{JTokenType.Null}", jToken[id]!["contents"]!.Type.ToString(),
                        "The content's type does not match the given type in Type key.");
                }
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException(nameof(Value), "The Contents / Value key could not be found.");
            }
        }
    }
}