using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Net.Exceptions;

namespace Settings.Net.SettingsEntry {
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
                    $"An ID cannot contain the character '{GetInvalidIdCharsInString(id)}'");
            }
            Value = value;
        }
        
        public override string ID { get; }
        public override string Description { get; set; }
        /// <summary> The value of the entry. </summary>
        internal string Value { get; set; }

        // Check that the JToken passed is correct
        private static void EnsureJsonState(JToken jToken) {
            // Check for null
            if (jToken is null) throw new ArgumentNullException(nameof(jToken), "jToken cannot be null.");
            // Check the ID of entry
            var id = ((JObject) jToken).Properties().ToList()[0].Name;
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character '{GetInvalidIdCharsInString(id)}");
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
                if (jToken[id]["value"].Type != JTokenType.String && jToken["value"].Type != JTokenType.Null)
                    throw new InvalidEntryValueException(
                        $"{JTokenType.String}' or '{JTokenType.Null}", jToken["value"].Type.ToString(), 
                        "The value's type does not match the object's type.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("value",
                    "Essential key is missing in JSON.");
            }
        }
    }
}