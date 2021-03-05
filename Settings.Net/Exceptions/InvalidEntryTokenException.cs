#nullable enable
using System;

namespace Settings.Exceptions {
    /// <summary>
    /// Throw if essential keys do not exist in the JSON.
    /// </summary>
    [Serializable]
    public class InvalidEntryTokenException : Exception {

        private string? _missingKey;

        public InvalidEntryTokenException(string? missingKey, string? messages = null) : base(messages) => _missingKey = missingKey;

        public override string Message => $"{base.Message} Key '{_missingKey}' could not be found.";
    }
}