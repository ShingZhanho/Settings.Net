#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Exceptions;
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