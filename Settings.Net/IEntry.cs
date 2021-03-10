﻿#nullable enable

namespace Settings.Net
{
    public interface IEntry
    {
        /// <summary>
        /// The ID of the entry. IDs shall not contain any illegal characters.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Description of the entry. Description can be null.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// The type of the entry.
        /// </summary>
        EntryType Type { get; }

        /// <summary>
        /// The parent of the entry. If the entry is a root group, this property will be null.
        /// </summary>
        SettingsGroup? Parent { get; }
        
        /// <summary>
        /// Gets the path to the current entry.
        /// </summary>
        string Path { get; }
        
        /// <summary>
        /// Gets the root of the current entry. Null is returned if the current entry has not yet been added to any group that has a root.
        /// </summary>
        SettingsGroup? Root { get; }
    }

    public interface IEntry<out TValue> : IEntry
    {
        /// <summary>
        /// The value of the entry. A list of objects when the entry is a group.
        /// </summary>
        TValue? Value { get; }
    }

    public enum EntryType
    {
        String,
        Int,
        Bool,
        Group
    }
}