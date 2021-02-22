using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Net.Exceptions;

namespace Settings.Net.SettingsEntry {
    /// <summary>
    /// Represents an entry with integer value.
    /// </summary>
    public sealed class IntEntry : BaseEntry {
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
    }
}