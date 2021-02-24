using System;
using Settings.Exceptions;

namespace Settings.SettingsEntry {
    /// <summary>
    /// Represents an entry with bool value.
    /// </summary>
    public sealed class BoolEntry : BaseEntry {
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
        
        public override string ID { get; }
        public override string Description { get; set; }
        public bool? Value { get; set; }
        public override string ToString() => Value is null ? string.Empty : Value.ToString();
    }
}