#nullable enable
using System;

namespace Settings.Exceptions {
    /// <summary>
    /// Throws if the entry type specified in the JSON does not match the object's type.
    /// </summary>
    [Serializable]
    public class EntryTypeNotMatchException : Exception {
        private string? _expectedType, _receivedType;

        public EntryTypeNotMatchException(string? expectedType, string? receivedType, string? messages = null) :
            base(messages) {
            _expectedType = expectedType;
            _receivedType = receivedType;
        }

        public override string Message => $"{base.Message} Expected '{_expectedType}', got '{_receivedType}'";
    }
}