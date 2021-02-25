using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Settings.Exceptions;

namespace Settings.SettingsEntry {
    /// <summary> Represents a group of Settings. </summary>
    public class EntriesGroup : BaseEntry, IEnumerable<BaseEntry> {
        private List<BaseEntry> _entries = new();

        public BaseEntry this[string id] =>
            !IdIsValid(id)
                ? throw new InvalidNameException(id,
                    $"An ID may not contain these characters: '{GetInvalidIdCharsInString(id)}'")
                : _entries.FirstOrDefault(entry => entry.ID == id);
        
        // Enables the class for being used in foreach blocks
        // IEnumerator and IEnumerable
        IEnumerator<BaseEntry> IEnumerable<BaseEntry>.GetEnumerator() => new EntriesGroupEnumerator(_entries);
        public IEnumerator GetEnumerator() => new EntriesGroupEnumerator(_entries);

        /// <summary>
        /// Enables class to be used within foreach.
        /// </summary>
        private class EntriesGroupEnumerator : IEnumerator<BaseEntry> {
            private readonly List<BaseEntry> _baseEntries;
            private int _position = -1;

            public EntriesGroupEnumerator(List<BaseEntry> entries) => _baseEntries = entries;
            public bool MoveNext() {
                _position++;
                return _position < _baseEntries.Count;
            }
            public void Reset() => _position = -1;
            public BaseEntry Current => _baseEntries[_position];
            object IEnumerator.Current => Current;
            public void Dispose() { }
        }

        public override string ID { get; }
        public override string Description { get; set; }
    }
}