#nullable enable
using System;
using System.IO;

namespace Settings.Net.Exceptions {
    [Serializable]
    public class InvalidNameException : Exception {

        private string? _invalidName;
        
        public InvalidNameException(){ }

        public InvalidNameException(string invalidName) {
            _invalidName = invalidName;
        }

        public InvalidNameException(string? invalidName, string? messages) : base(messages){
            _invalidName = invalidName;
        }

        public override string Message {
            get {
                var s = base.Message;
                if (!string.IsNullOrEmpty(_invalidName))
                    s += $" Name \"{_invalidName}\" is invalid.";
                return s;
            }
        }
    }
}