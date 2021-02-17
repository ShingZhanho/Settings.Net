#nullable enable
using System;

namespace Settings.Net.Exceptions {
    /// <summary>
    /// Throws if the type of value does not match the object's value type.
    /// </summary>
    [Serializable]
    public class InvalidEntryValueException : EntryTypeNotMatchException {
        public InvalidEntryValueException() { }

        public InvalidEntryValueException(string? expectedType, string? receivedType) : base(expectedType, receivedType) { }

        public InvalidEntryValueException(string? expectedType, string? receivedType, string? messages) : base(
            expectedType, receivedType, messages) { }
    }
}