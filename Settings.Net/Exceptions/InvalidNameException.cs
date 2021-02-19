#nullable enable
using System;
using System.IO;

namespace Settings.Net.Exceptions {
    /// <summary>
    /// Throw if the ID of an entry contains invalid characters.
    /// Invalid chars can be retrieved by calling <see cref="SettingsEntry.BaseEntry.GetInvalidIdChars"/>.
    /// </summary>
    [Serializable]
    public class InvalidNameException : Exception {

        private string? _invalidName;

        public InvalidNameException(string? invalidName, string? messages = null) : base(messages){
            _invalidName = invalidName;
        }

        public override string Message {
            get {
                var m = base.Message;
                if (!string.IsNullOrEmpty(_invalidName))
                    m += $" Name \"{_invalidName}\" is invalid.";
                return m;
            }
        }
    }
}