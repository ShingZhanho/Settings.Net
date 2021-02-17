#nullable enable
using System;

namespace Settings.Net.Exceptions {
    /// <summary>
    /// Throw if essential keys do not exist in the JSON.
    /// </summary>
    [Serializable]
    public class InvalidEntryTokenException : Exception {

        private string? _missingKey;
        
        public InvalidEntryTokenException() { }

        public InvalidEntryTokenException(string? missingKey) => _missingKey = missingKey;

        public InvalidEntryTokenException(string? missingKey, string messages) : base(messages) => _missingKey = missingKey;

        public override string Message {
            get {
                var m = base.Message;
                if (!string.IsNullOrEmpty(_missingKey))
                    m += $" Key '{_missingKey}' could not be found.";
                return m;
            }
        }
    }
}