using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Settings.Net.Exceptions;

#nullable enable

namespace Settings.Net
{
    public class SettingsBundle
    {
        /// <summary>
        /// Initializes a new SettingsBundle with no values.
        /// </summary>
        public SettingsBundle() { }
        
        /// <summary>
        /// Initializes a SettingsBundle from an existing .sbd file.
        /// </summary>
        /// <param name="filePath">The path to the .sbd file.</param>
        public SettingsBundle(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file could not be found.", filePath);
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(filePath);
                _ = JObject.Parse(jsonString); // Validate JSON
            }
            catch (JsonReaderException e)
            { // Invalid Json
                throw new JsonReaderException("Not a valid JSON data.", e);
            }
            catch (Exception e)
            { // Error while reading JSON.
                throw new IOException("Error while trying to read the file.", e);
            }
            PrivateConstructor(jsonString);
        }

        public SettingsBundle(JToken jToken) => PrivateConstructor(jToken);

        public SettingsGroup this[string id]
        {
            get
            {
                foreach (var item in Roots.Where(item => item.Id == id))
                    return item;
                throw new IndexOutOfRangeException($"The specified key could not be found. ID:'{id}'");
            }
        }

        public bool ContainsKey(string key) => Roots.Any(item => item.Id == key);

        public string? Description { get; set; }
        public List<SettingsGroup> Roots { get; private set; } = new ();

        /// <summary>
        /// Adds a existing group as a root to the current bundle.
        /// </summary>
        /// <param name="root">The root to be added.</param>
        /// <returns>The path of the successfully added root.</returns>
        /// <exception cref="InvalidOperationException">Throws if a root with the same ID already exists.</exception>
        public string AddNewRoot(SettingsGroup root)
        {
            if (ContainsKey(root.Id))
                throw new InvalidOperationException($"A group with the same ID '{root.Id}' already exists.");
            root.IsRoot = true;
            Roots.Add(root);
            return this[root.Id].Path;
        }

        /// <summary>
        /// Adds a new group as a root to the current bundle.
        /// </summary>
        /// <param name="id">The ID of the new group.</param>
        /// <param name="members">The member items of the group.</param>
        /// <param name="description">The description of the group.</param>
        /// <returns>The path of the successfully added group.</returns>
        /// <exception cref="InvalidOperationException">Throws if a root with the same ID already exists.</exception>
        public string AddNewRoot(string id, List<IEntryNode>? members = null, string? description = null) =>
            AddNewRoot(new SettingsGroup(id, members, true) {Description = description});

        private void PrivateConstructor(JToken jToken)
        {
            // Construct a new object with JToken
            InternalEnsureJsonState(jToken);
            Description = jToken["metadata"]!["desc"]!.ToString();
            // Add groups
            foreach (var root in jToken["data"]!)
            {
                var group = new SettingsGroup(root);
                AddNewRoot(group);
            }
        }

        private static void InternalEnsureJsonState(JToken jToken)
        {
            // Check json data for errors
            // Check "metadata" key
            if (!((JObject) jToken).ContainsKey("metadata"))
                throw new InvalidEntryTokenException("metadata", "The file does not have the essential key 'metadata'");
            // Check "data" key
            if (!((JObject) jToken).ContainsKey("data"))
                throw new InvalidEntryTokenException("data", "The file does not have the essential key 'data'");
        }
    }
}