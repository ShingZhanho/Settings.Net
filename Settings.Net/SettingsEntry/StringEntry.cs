using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Settings.Net.SettingsEntry {
    /// <summary>
    /// Represents an entry with string value.
    /// </summary>
    public class StringEntry : BaseEntry {
        /// <summary>
        /// Initializes a StringEntry object from existing JSON data.
        /// </summary>
        /// <param name="jToken">The existing JToken</param>
        internal StringEntry(JToken jToken) {
            
        }

        /// <summary>
        /// Initializes a new StringEntry object.
        /// </summary>
        /// <param name="ID">The ID of the new entry.</param>
        /// <param name="value">The value of the new entry.</param>
        internal StringEntry(string ID, string value) {
            
        }
        
        public override string ID { get; }
        public override string Description { get; }
        /// <summary>
        /// The value of the entry
        /// </summary>
        public string Value { get; set; }
    }
}