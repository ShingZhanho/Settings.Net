using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Exceptions;

namespace Settings.SettingsEntry {
    /// <summary>
    /// Represents an entry with integer value.
    /// </summary>
    public sealed class IntEntry : BaseEntry {
        /// <summary>
        /// Initializes an IntEntry from existing JSON data.
        /// </summary>
        /// <param name="jToken">The existing JToken.</param>
        internal IntEntry(JToken jToken) {
            EnsureJsonState(jToken);

            ID = ((JObject) jToken).Properties().ToList()[0].Name;
            Value = int.Parse(jToken[ID]["value"].ToString());
            Description = jToken[ID]["desc"].ToString();
        }

        /// <summary>
        /// Initializes a new IntEntry object.
        /// </summary>
        /// <param name="id">The ID of the new entry.</param>
        /// <param name="value">The value of the new entry.</param>
        internal IntEntry(string id, int value) {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "ID cannot be null or empty.");
            // Check for illegal chars in id
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character(s) '{GetInvalidIdCharsInString(id)}'");
            ID = id;
            Value = value;
        }
        
        public override string ID { get; }
        public override string Description { get; set; }
        public int Value { get; set; }

        public override string ToString() => Value.ToString();
        
        // Check that the JToken passed is correct and valid
        private static void EnsureJsonState(JToken jToken) {
            // Check the ID of entry
            var id = ((JObject) jToken).Properties().ToList()[0].Name;
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain the character(s) '{GetInvalidIdCharsInString(id)}'");
            // Check entry type
            try {
                if (jToken[id]["type"].ToString() != Constants.EntryTypeFlags.IntEntry)
                    throw new EntryTypeNotMatchException(Constants.EntryTypeFlags.IntEntry,
                        jToken[id]["type"].ToString(), "Type of entry does not match.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("type", "Essential key is missing in JSON.");
            }
            // Check entry value
            try {
                if (jToken[id]["value"].Type != JTokenType.Integer && jToken[id]["value"].Type != JTokenType.Null)
                    throw new InvalidEntryValueException(
                        $"{JTokenType.Integer} or {JTokenType.Null}", jToken[id].Type.ToString(),
                        "The value's type does not match the object's type.");
            } catch (NullReferenceException) {
                throw new InvalidEntryTokenException("value",
                    "Essential key is missing in JSON.");
            }
        }
    }
}