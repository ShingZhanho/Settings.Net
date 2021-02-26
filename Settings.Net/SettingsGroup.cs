﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Settings.Net.Exceptions;

namespace Settings.Net {
    /// <summary>
    /// Represents a group of entries.
    /// </summary>
    public class SettingsGroup : IEntryNode<List<IEntryNode>> {
        /// <summary>
        /// Initializes a new root of settings.
        /// </summary>
        /// <param name="id">The ID of the root.</param>
        /// <param name="childrenItems">A list of children items which belongs this root.</param>
        public SettingsGroup(string id, List<IEntryNode> childrenItems) {
            // Check ID status
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain these characters: '{GetInvalidIdCharsInString(id)}'");
            Id = id;
            Value = childrenItems;
            IsRoot = true;
            Type = EntryType.Group;
        }

        public string Id { get; }
        public string? Description { get; }
        public EntryType Type { get; }
        public SettingsGroup? Parent { get; }
        public List<IEntryNode> Value { get; }
        /// <summary>
        /// Indicating whether the current group is a root.
        /// </summary>
        public bool IsRoot { get; }

        public IEntryNode this[string id] {
            get {
                foreach (var item in Value.Where(item => item.Id == id))
                    return item;
                throw new ArgumentOutOfRangeException(nameof(id), "Specified ID could not be found.");
            }
        }
        
        /// <summary>Gets an array of invalid ID characters.</summary>
        public static char[] InvalidIdChars {
            get { return new[] {'.', ' '}; }
        }

        /// <summary> Checks if the given ID is a valid ID.</summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>true if valid, false instead.</returns>
        internal static bool IdIsValid(string id) => id.IndexOfAny(InvalidIdChars) == -1;

        /// <summary> Gets the list of chars that are illegal to use for an ID name. </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdCharsInString(string str) => str.Where(character => InvalidIdChars.Contains(character)).ToArray();
    }
    
    /// <summary>Represents an entry with value.</summary>
    public class SettingEntry<TValue> : IEntryNode<TValue> {
        public SettingEntry(string id, TValue? value) {
            // Check for accepted types
            if (!new[] {typeof(string), typeof(int), typeof(bool)}.Contains(typeof(TValue)))
                throw new ArgumentOutOfRangeException(nameof(TValue), "This type is not accepted." +
                                                                 " Accepts only string, int and bool");
            // Check for illegal chars in ID
            if (!IdIsValid(id))
                throw new InvalidNameException(nameof(id), "IDs should not contain chars: " +
                                                           $"'{GetInvalidIdCharsInString(id)}'");
            // Assign Type property
            var enumDict = new Dictionary<Type, EntryType>();
            enumDict.Add(typeof(string), EntryType.String);
            enumDict.Add(typeof(int), EntryType.Int);
            enumDict.Add(typeof(bool), EntryType.Bool);
            Type = enumDict[typeof(TValue)];
            // Assign properties
            Id = id;
            Value = value;
        }
        
        public string Id { get; }
        public string? Description { get; set; }
        public TValue? Value { get; }
        public EntryType Type { get; }
        public SettingsGroup? Parent { get; }

        /// <summary>Gets an array of invalid ID characters.</summary>
        public static char[] InvalidIdChars {
            get { return new[] {'.', ' '}; }
        }

        /// <summary> Checks if the given ID is a valid ID.</summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>true if valid, false instead.</returns>
        internal static bool IdIsValid(string id) => id.IndexOfAny(InvalidIdChars) == -1;

        /// <summary> Gets the list of chars that are illegal to use for an ID name. </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdCharsInString(string str) => str.Where(character => InvalidIdChars.Contains(character)).ToArray();
    }
}