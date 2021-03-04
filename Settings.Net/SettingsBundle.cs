using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Settings.Net.Exceptions;
using JetBrains.Annotations;

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

        /// <summary>
        /// Removes a group from the bundle by its ID.
        /// </summary>
        /// <param name="rootId">The ID of the entry to remove.</param>
        /// <param name="recursive">Indicating whether to remove the group recursively.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the given ID does not match any group.</exception>
        /// <exception cref="InvalidOperationException">Throws if the group being removed has children but parameter recursive is set to false.</exception>
        public void RemoveRoot(string rootId, bool recursive = false)
        {
            try
            {
                _ = this[rootId];
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("The specified ID could not be found.", e);
            }
            if (this[rootId].HasChildren && !recursive)
                throw new InvalidOperationException(
                    "Unable to remove a group contains children with parameter 'recursive' set to false.");
            Roots.Remove(this[rootId]);
        }

        /// <summary>
        /// Gets the entry by path.
        /// </summary>
        /// <param name="path">The path of the entry to get.</param>
        /// <returns>The specified entry. null is returned if not found.</returns>
        /// <exception cref="InvalidOperationException">Throws if part of the path points to an entry rather than a group.</exception>
        public IEntryNode? GetEntryByPath(string path)
        {
            IEntryNode currEntry;
            try
            {
                currEntry = this[path.Split('.')[0]];
                for (var index = 1; index < path.Split('.').Length - 1; index++)
                {
                    if (currEntry.Type != EntryType.Group)
                        throw new InvalidOperationException(
                            $"This part of the path points to an entry, not a group: '{currEntry.Path}'");
                    currEntry = ((SettingsGroup)currEntry)[path.Split('.')[index]];
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            return currEntry;
        }

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

        [AssertionMethod]
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