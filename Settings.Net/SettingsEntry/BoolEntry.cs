using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Exceptions;

namespace Settings.SettingsEntry {
    /// <summary>
    /// Represents an entry with bool value.
    /// </summary>
    public sealed class BoolEntry : BaseEntry {
        /// <summary>
        /// Initializes a BoolEntry from existing JSON data.
        /// </summary>
        /// <param name="jToken">The existing JToken.</param>
        internal BoolEntry(JToken jToken) {
            EnsureJsonState(jToken);
            
            ID = ((JObject) jToken).Properties().ToList()[0].Name;
            Value = (bool) jToken[ID]["value"];
            Description = jToken[ID]["desc"].ToString();
        }
        
        /// <summary>
        /// Initializes a new BoolEntry object.
        /// </summary>
        /// <param name="id">The ID of the new entry.</param>
        /// <param name="value">The value of the new entry.</param>
        internal BoolEntry(string id, bool? value) {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "ID can be neither null nor empty.");
            // Check for illegal chars in id
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character(s) '{GetInvalidIdCharsInString(id)}'");
            ID = id;
            Value = value;
        }
        
        // Checks the jToken passed in:
        private static void EnsureJsonState(JToken jToken) {
            // Check the ID of entry
            var id = ((JObject) jToken).Properties().ToList()[0].Name;
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character(s) '{GetInvalidIdCharsInString(id)}'");
            // Check entry type
            try {
                if (jToken[id]["type"].ToString() != Constants.EntryTypeFlags.BoolEntry)
                    throw new EntryTypeNotMatchException(Constants.EntryTypeFlags.BoolEntry,
                        jToken[id]["type"].ToString(), "Type of entry does not match.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("type", "Essential key is missing in JSON.");
            }
            // Check entry value
            try {
                if (jToken[id]["value"].Type != JTokenType.Boolean && jToken[id]["value"].Type != JTokenType.Null)
                    throw new InvalidEntryValueException(
                        $"{JTokenType.Boolean} or {JTokenType.Null}", jToken[id].Type.ToString(),
                        "The value's type does not match the object's type.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("value",
                    "Essential key is missing in JSON.");
            }
        }
        
        public override string ID { get; }
        public override string Description { get; set; }
        public bool? Value { get; set; }
        public override string ToString() => Value is null ? string.Empty : Value.ToString();
    }
}