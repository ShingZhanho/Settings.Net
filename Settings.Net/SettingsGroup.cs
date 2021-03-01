#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Net.Exceptions;

namespace Settings.Net
{
    /// <summary>
    /// Represents a group of entries.
    /// </summary>
    public class SettingsGroup : IEntryNode<List<IEntryNode>>
    {
        /// <summary>
        /// Initializes a new group of settings.
        /// </summary>
        /// <param name="id">The ID of the root.</param>
        /// <param name="childrenItems">A list of children items which belongs this root.</param>
        /// <param name="isRoot">Indicates whether this entry is a root.</param>
        public SettingsGroup(string id, List<IEntryNode> childrenItems, bool isRoot = false)
        {
            // Check ID status
            if (!IdIsValid(id))
                throw new InvalidNameException(id,
                    $"An ID cannot contain these characters: '{GetInvalidIdCharsInString(id)}'");
            Id = id;
            Value = childrenItems;
            IsRoot = isRoot;
        }

        /// <summary>
        /// Initializes a new group of entries from existing JSON data.
        /// </summary>
        /// <param name="jToken">The parsed JSON.</param>
        /// <param name="isRoot">Indicating whether the entry is a root.</param>
        public SettingsGroup(JToken jToken, bool isRoot = false)
        {
            InternalEnsureJsonState(jToken);

            Id = ((JObject) jToken).Properties().ToList()[0].Name;
            Description = jToken[Id]!["desc"]!.ToString();
            var valueList = new List<IEntryNode>();
            // Initializes each item
            foreach (var subItem in jToken[Id]!["contents"]!)
            {
                var subId = ((JObject) subItem).Properties().ToList()[0].Name;
                // If the item is a group
                if (subItem[subId]!["type"]!.ToString() == EntryType.Group.ToString())
                    valueList.Add(new SettingsGroup(subItem));
                // If the item is not a group
                valueList.Add(subItem[subId]!["type"]!.ToString() switch
                {
                    nameof(EntryType.String) => new SettingEntry<string>(subItem),
                    nameof(EntryType.Int) => new SettingEntry<int>(subItem),
                    nameof(EntryType.Bool) => new SettingEntry<bool>(subItem),
                    _ => throw new ArgumentOutOfRangeException(null, "Unknown Type.")
                });
            }

            Value = valueList;
            IsRoot = isRoot;
        }

        public string Id { get; }
        public string? Description { get; }
        public EntryType Type { get; } = EntryType.Group; // fixed
        public SettingsGroup? Parent { get; internal set; }
        public List<IEntryNode> Value { get; }
        /// <summary>
        /// Gets whether this group has any sub-groups or entries inside.
        /// </summary>
        public bool HasChildren => Value.Count > 0;

        /// <summary>
        /// Indicating whether the current group is a root.
        /// </summary>
        public bool IsRoot { get; }

        public IEntryNode this[string id]
        {
            get
            {
                foreach (var item in Value.Where(item => item.Id == id))
                    return item;
                throw new ArgumentOutOfRangeException(nameof(id), "Specified ID could not be found.");
            }
        }

        /// <summary>
        /// Gets the path to the current group.
        /// </summary>
        public string Path => Parent == null ? Id : $"{Parent.Path}.{Id}";

        public SettingsGroup? Root
        {
            get
            {
                var rootGroup = this;
                while (!rootGroup.IsRoot)
                {
                    if (rootGroup.Parent == null) return null;
                    rootGroup = rootGroup.Parent;
                }
                return rootGroup;
            }
        }

        /// <summary>Gets an array of invalid ID characters.</summary>
        public static char[] InvalidIdChars
        {
            get { return new[] {'.', ' '}; }
        }

        /// <summary> Checks if the given ID is a valid ID.</summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>true if valid, false instead.</returns>
        internal static bool IdIsValid(string id) => id.IndexOfAny(InvalidIdChars) == -1;

        /// <summary> Gets the list of chars that are illegal to use for an ID name. </summary>
        /// <returns>List of invalid ID chars.</returns>
        public static char[] GetInvalidIdCharsInString(string str) =>
            str.Where(character => InvalidIdChars.Contains(character)).ToArray();

        /// <summary>
        /// Adds an entry or a group to the current group. The entry's ID must not be the same with the existing items'.
        /// </summary>
        /// <param name="entry">The entry or group object to add.</param>
        /// <returns>The path of the added entry.</returns>
        public string Add(IEntryNode entry)
        {
            if (this[entry.Id] != null)
                // Prevents adding an entry with duplicated ID
                throw new InvalidOperationException(
                    $"An entry with the same ID '{entry.Id}' already exists in this group.");
            Value.Add(entry);
            return this[entry.Id].Path;
        }

        /// <summary>
        /// Adds an new entry to the current group. The entry's ID must not be the same with the existing items'.
        /// </summary>
        /// <param name="id">The ID of the entry to add.</param>
        /// <param name="value">The value of the new entry.</param>
        /// <param name="description">The description of the new entry.</param>
        /// <returns>The path of the added entry.</returns>
        public string Add<T>(string id, T value, string? description = null) => Add(new SettingEntry<T>(id, value){Description = description});

        /// <summary>
        /// Adds a new group with or without existing members. The group's ID must not be the same with the existing items'.
        /// </summary>
        /// <param name="id">The ID of the new group.</param>
        /// <param name="children">The children items of this group.</param>
        /// <returns>The path of the new added group.</returns>
        public string AddGroup(string id, List<IEntryNode>? children = null) =>
            Add(new SettingsGroup(id, children ?? new List<IEntryNode>()));

        /// <summary>
        /// Removes an entry from the current group.
        /// </summary>
        /// <param name="id">The id of the entry to remove.</param>
        public void RemoveEntry(string id)
        {
            if (this[id] == null)
                throw new ArgumentOutOfRangeException(nameof(id), "The specified ID could not be found.");
            if (this[id].Type == EntryType.Group)
                throw new InvalidOperationException(
                    "The specified ID is a group and cannot be removed with RemoveEntry(). Use RemoveGroup() instead");
            Value.Remove(this[id]);
        }

        /// <summary>
        /// Removes a group from the current group.
        /// </summary>
        /// <param name="id">The ID of the group to remove.</param>
        /// <param name="recursive">Indicating whether remove the group recursively if the group has children.</param>
        /// <exception cref="InvalidOperationException">Throws if the group has children but 'recursive' is false.</exception>
        public void RemoveGroup(string id, bool recursive = false)
        {
            if (this[id] == null)
                throw new ArgumentOutOfRangeException(nameof(id), "The specified ID could not be found.");
            if (this[id].Type != EntryType.Group)
                throw new InvalidOperationException(
                    "The specified ID is not a group and cannot be removed with RemoveGroup(). Use RemoveEntry() instead");
            if (((SettingsGroup) this[id]).HasChildren && !recursive)
                // Has values but recursive option is false
                throw new InvalidOperationException(
                    "The group specified has values. Set parameter 'recursive' to true to remove this group recursively.");
            Value.Remove(this[id]);
        }

        /// <summary>
        /// Gets the index of the specified entry.
        /// </summary>
        /// <param name="entry">The entry to get.</param>
        public int IndexOf(IEntryNode entry) =>
            !Value.Contains(entry)
                ? throw new ArgumentException("This entry does not belong to the current group.", nameof(entry))
                : Value.IndexOf(entry);

        /// <summary>
        /// Gets the index of the specified entry by ID.
        /// </summary>
        /// <param name="entryId">The ID of the entry to get.</param>
        public int IndexOf(string entryId)
        {
            try
            {
                var entryToGet = this[entryId];
                return IndexOf(entryToGet);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentException("The specified entry could not be found under this group.", nameof(entryId),
                    e);
            }
        }

        // Check JSON before constructing object
        private static void InternalEnsureJsonState(JToken jToken)
        {
            // Check ID
            string id;
            try
            {
                id = ((JObject) jToken).Properties().ToList()[0].Name;
                if (!IdIsValid(id))
                    throw new InvalidNameException(id,
                        $"The ID '{id}' is invalid. These characters are illegal: '{GetInvalidIdCharsInString(id)}'");
            }
            catch (NullReferenceException)
            {
                throw new InvalidEntryTokenException(nameof(Id), "The ID key could not be found in the JSON.");
            }

            // Check entry type
            try
            {
                if (jToken[id]!["type"]!.ToString() != EntryType.Group.ToString())
                {
                    var enumParseSuccess =
                        Enum.TryParse(typeof(EntryType), jToken[id]!["type"]!.ToString(), out var result);
                    throw enumParseSuccess
                        ? new EntryTypeNotMatchException(EntryType.Group.ToString(), result!.ToString(),
                            "Passed in JToken is not an entry group.")
                        : new ArgumentOutOfRangeException(nameof(jToken), $"Unknown type: {jToken[id]!["type"]!}");
                }
            }
            catch (NullReferenceException)
            {
                throw new InvalidEntryTokenException(nameof(Type), "The Type key could not be found.");
            }

            // Check entry value
            try
            {
                if (jToken[id]!["contents"]!.Type != JTokenType.Array &&
                    jToken[id]!["contents"]!.Type != JTokenType.Null)
                {
                    throw new InvalidEntryValueException(
                        $"{JTokenType.Array}' or '{JTokenType.Null}", jToken[id]!["contents"]!.Type.ToString(),
                        "The content's type does not match the given type in Type key.");
                }
            }
            catch (NullReferenceException)
            {
                throw new InvalidEntryTokenException(nameof(Value), "The Contents / Value key could not be found.");
            }
        }
    }
}