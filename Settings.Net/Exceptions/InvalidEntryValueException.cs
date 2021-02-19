#nullable enable
using System;

namespace Settings.Net.Exceptions {
    /// <summary>
    /// Throws if the type of value does not match the object's value type.
    /// </summary>
    [Serializable]
    public class InvalidEntryValueException : EntryTypeNotMatchException {
        public InvalidEntryValueException(string? expectedType, string? receivedType, string? messages = null) : base(
            expectedType, receivedType, messages) { }
    }
}