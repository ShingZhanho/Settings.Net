#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Settings.Net.Exceptions;

#pragma warning disable 8714

namespace Settings.Net
{
    /// <summary>Represents an entry with value.</summary>
    public sealed class SettingEntry<TValue> : AbstractEntry<TValue>
    {
        public SettingEntry(string id, TValue? value)
        {
            // Check for accepted types
            if (!new[] {typeof(string), typeof(int), typeof(bool)}.Contains(typeof(TValue)))
                throw new ArgumentOutOfRangeException(nameof(TValue), "This type is not accepted." +
                                                                      " Accepts only string, int and bool");
            // Check for illegal chars in ID
            if (!IdIsValid(id))
                throw new InvalidNameException(nameof(id), "IDs should not contain chars: " +
                                                           $"'{GetInvalidIdCharsInString(id)}'");
            // Assign Type property
            Type = GetTypeEnum<TValue>() ?? throw new ArgumentOutOfRangeException(nameof(TValue), "This type is not accepted");
            // Assign properties
            Id = id;
            Value = value;
        }

        public SettingEntry(JToken jToken)
        {
            InternalEnsureJsonState(jToken);

            Id = ((JObject) jToken).Properties().ToList()[0].Name;
            Type = GetTypeEnum(jToken[Id]!["type"]!.Type)
                   ?? throw new ArgumentOutOfRangeException(jToken[Id]!["type"]!.Type.ToString(),
                       "This type is not accepted.");
            // Assign value
            try
            {
                // Try convert the json value to the TValue type.
                Value = jToken[Id]!["value"]!.Type == JTokenType.Null
                    ? default
                    : (TValue) Convert.ChangeType(jToken[Id]!["value"]!.ToString(), typeof(TValue));
            }
            catch (FormatException)
            {
                Console.WriteLine("Failed to convert values.");
                throw;
            }
            Description = jToken[Id]!["desc"] is null
                ? null
                : jToken[Id]!["desc"]!.ToString();
        }

        public override string Id { get; }
        public override string? Description { get; set; }
        public override TValue? Value { get; internal set; }
        public override EntryType Type { get; }
        public override SettingsGroup? Parent { get; internal set; }
        public override string Path => Parent == null ? Id : $"{Parent.Path}.{Id}";
        [Obsolete("This is obsolete.", true)]
        public override AbstractEntry this[string id] =>
            throw new NotImplementedException("Using indexer of an entry is invalid. This will be refactored later.");

        public override SettingsGroup? Root
        {
            get
            {
                if (Parent == null) return null;
                var rootGroup = this.Parent;
                while (!rootGroup.IsRoot)
                {
                    if (rootGroup.Parent == null) return null;
                    rootGroup = Parent;
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
        /// Gets the corresponding enum of the given type. Null is returned if no enum matches.
        /// </summary>
        private static EntryType? GetTypeEnum<T>()
        {
            var enumDict = new Dictionary<Type, EntryType>
            {
                {typeof(string), EntryType.String}, {typeof(int), EntryType.Int}, {typeof(bool), EntryType.Bool}
            };
            return enumDict.ContainsKey(typeof(T))
                ? enumDict[typeof(T)]
                : null;
        }

        /// <summary>
        /// Gets the corresponding enum of the given JTokenType. Null is returned if no enum matches.
        /// </summary>
        private static EntryType? GetTypeEnum(JTokenType type) =>
            type switch
            {
                JTokenType.String => GetTypeEnum<string>(),
                JTokenType.Integer => GetTypeEnum<int>(),
                JTokenType.Boolean => GetTypeEnum<bool>(),
                _ => null
            };

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
            var typeDictionary = new Dictionary<Type, EntryType>
            {
                {typeof(string), EntryType.String}, {typeof(int), EntryType.Int}, {typeof(bool), EntryType.Bool}
            };
            try
            {
                if (jToken[id]!["type"]!.ToString() != typeDictionary[typeof(TValue)].ToString())
                {
                    var enumParseSuccess =
                        Enum.TryParse(typeof(EntryType), jToken[id]!["type"]!.ToString(), out var result);
                    throw enumParseSuccess
                        ? new EntryTypeNotMatchException(typeDictionary[typeof(TValue)].ToString(), result!.ToString(),
                            $"Passed in JToken is not a {typeDictionary[typeof(TValue)].ToString()} entry.")
                        : new ArgumentOutOfRangeException(nameof(jToken), $"Unknown type: {jToken[id]!["type"]!}");
                }
            }
            catch (NullReferenceException)
            {
                throw new InvalidEntryTokenException(nameof(Type), "The Type key could not be found.");
            }

            // Check entry value
            var tokenTypeDictionary = new Dictionary<Type, JTokenType>
            {
                {typeof(string), JTokenType.String}, {typeof(int), JTokenType.Integer},
                {typeof(bool), JTokenType.Boolean}
            };
            try
            {
                if (jToken[id]!["value"]!.Type != tokenTypeDictionary[typeof(TValue)] &&
                    jToken[id]!["value"]!.Type != JTokenType.Null)
                {
                    throw new InvalidEntryValueException(
                        $"{tokenTypeDictionary[typeof(TValue)]}' or '{JTokenType.Null}",
                        jToken[id]!["value"]!.Type.ToString(),
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