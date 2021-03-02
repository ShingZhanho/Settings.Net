using System;
using System.Collections.Generic;
using System.IO;
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

        public string? Description { get; set; }
        public List<SettingsGroup> Roots { get; private set; } = new ();

        private void PrivateConstructor(JToken jToken)
        {
            // Construct a new object with JToken
            InternalEnsureJsonState(jToken);
            Description = jToken["metadata"]!["desc"]!.ToString();
            
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