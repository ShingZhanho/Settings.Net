#nullable enable

namespace Settings.Net
{
    public abstract class IEntry
    {
        /// <summary>
        /// The ID of the entry. IDs shall not contain any illegal characters.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Description of the entry. Description can be null.
        /// </summary>
        public abstract string? Description { get; set; }

        /// <summary>
        /// The type of the entry.
        /// </summary>
        public abstract EntryType Type { get; }

        /// <summary>
        /// The parent of the entry. If the entry is a root group, this property will be null.
        /// </summary>
        public abstract SettingsGroup? Parent { get; internal set; }
        
        /// <summary>
        /// Gets the path to the current entry.
        /// </summary>
        public abstract string Path { get; }
        
        /// <summary>
        /// Gets the root of the current entry. Null is returned if the current entry has not yet been added to any group that has a root.
        /// </summary>
        public abstract SettingsGroup? Root { get; }
        
        public abstract IEntry this[string id] { get; }
    }

    public abstract class IEntry<TValue> : IEntry
    {
        /// <summary>
        /// The value of the entry. A list of objects when the entry is a group.
        /// </summary>
        public abstract TValue? Value { get; internal set;  }
    }

    public enum EntryType
    {
        String,
        Int,
        Bool,
        Group
    }
}