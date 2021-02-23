using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Exceptions;

namespace Settings.SettingsEntry {
    /// <summary> Represents an entry with string value. </summary>
    public sealed class StringEntry : BaseEntry {
        /// <summary> Initializes a StringEntry object from existing JSON data. </summary>
        /// <param name="jToken">The existing JToken</param>
        internal StringEntry(JToken jToken) {
            EnsureJsonState(jToken);

            ID = ((JObject) jToken).Properties().ToList()[0].Name;
            Value = jToken[ID]["value"].ToString();
            Description = jToken[ID]["desc"].ToString();
        }

        /// <summary> Initializes a new StringEntry object. </summary>
        /// <param name="id">The ID of the new entry.</param>
        /// <param name="value">The value of the new entry.</param>
        internal StringEntry(string id, string value) {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "ID cannot be null or empty.");
            // Check for illegal chars in the id
            if (id.IndexOfAny(InvalidIdChars) == -1) {
                ID = id;
            } else {
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character(s) '{GetInvalidIdCharsInString(id)}'");
            }
            Value = value;
        }
        
        public override string ID { get; }
        public override string Description { get; set; }
        /// <summary> The value of the entry. </summary>
        public string Value { get; set; }

        /// <summary>Returns the string of the value of this entry.</summary>
        public override string ToString() => Value;

        // Check that the JToken passed is correct
        private static void EnsureJsonState(JToken jToken) {
            // Check the ID of entry
            var id = ((JObject) jToken).Properties().ToList()[0].Name;
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character(s) '{GetInvalidIdCharsInString(id)}");
            // Check entry type
            try {
                if (jToken[id]["type"].ToString() != Constants.EntryTypeFlags.StringEntry)
                    throw new EntryTypeNotMatchException(Constants.EntryTypeFlags.StringEntry,
                        jToken[id]["type"].ToString(), "Type of entry does not match.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("type", "Essential key is missing in JSON.");
            }
            // Check entry value
            try {
                if (jToken[id]["value"].Type != JTokenType.String && jToken[id]["value"].Type != JTokenType.Null)
                    throw new InvalidEntryValueException(
                        $"{JTokenType.String}' or '{JTokenType.Null}", jToken[id]["value"].Type.ToString(), 
                        "The value's type does not match the object's type.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("value",
                    "Essential key is missing in JSON.");
            }
        }
    }
}