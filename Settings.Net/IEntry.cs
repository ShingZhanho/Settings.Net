#nullable enable

namespace Settings.Net {
    public interface IEntry {
        /// <summary>
        /// The ID of the entry. IDs shall not contain any illegal characters.
        /// </summary>
        string Id { get; }
        EntryType Type { get; }
        IEntry? Parent { get; }
    }
    
    public interface IEntry<out TValue> : IEntry {
        TValue Value { get; }
    }
    
    public enum EntryType {
        String,
        Int,
        Bool,
        Group
    }
}