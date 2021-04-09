using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Settings.Net.Exceptions;

namespace Settings.Net.Tests
{
    public class SettingEntryTests
    {
        [Test,
         Description("Calling a SettingEntry's indexer always gets null.")]
        public void Indexer_AlwaysNull()
        {
            // Arrange
            var entry = new SettingEntry<int>(JToken.Parse(File.ReadAllText(TestData.SettingEntry.NormalIntEntryJsonPath)));
            
            // Act
            var results = entry["something"];
            
            // Assert
            Assert.That(results, Is.Null);
        }

        [TestCase("stringEntry", "value", "This is a string entry."),
         TestCase("intEntry", 689),
         TestCase("boolEntry", false, "This is a bool entry.")]
        public void Ctor_Data_Successful<T>(string id, T value, string description = null)
        {
            // Act
            var result = new SettingEntry<T>(id, value) {Description = description};
            
            // Assert
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Value, Is.EqualTo(value));
            Assert.That(result.Description, Is.EqualTo(description));
        }
        
        [TestCase("invalid.string.id", "value"),
         TestCase("some more invalid id", false),
         TestCase("even more.id", 5),
         Description("Initialize a new SettingEntry instance with an invalid ID. InvalidNameException is expected.")]
        public void Ctor_InvalidId_InvalidNameException<T>(string id, T value) 
            => Assert.Throws<InvalidNameException>(() => _ = new SettingEntry<T>(id, value)); // Assert

        [Test,
         Description(
             "Initialize a new SettingEntry instance with a type other than string, int or bool. ArgumentOutOfRangeException is expected.")]
        public void Ctor_UnacceptedType_ArgumentOutOfRangeException()
        {
            // Arrange
            var value = new[] {"string1", "string2"};
            
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SettingEntry<string[]>("id", value));
        }
        
        public static IEnumerable<TestCaseData> InvalidIdTestCases
        {
            get
            {
                yield return new TestCaseData(TestData.SettingEntry.StringEntryWithInvalidIdJsonPath);
                yield return new TestCaseData(TestData.SettingEntry.IntEntryWithInvalidIdJsonPath);
                yield return new TestCaseData(TestData.SettingEntry.BoolEntryWithInvalidJsonPath);
            }
        }
        
        [TestCaseSource(nameof(InvalidIdTestCases)),
         Description("Attempts to initialize a new entry with invalid ID, InvalidNameException is expected.")]
        public void Ctor_InvalidIdJsonFile_InvalidNameException(string jsonFile)
        {
            // Arrange
            var jToken = JToken.Parse(File.ReadAllText(jsonFile));
            var type = jToken
                [((JObject) jToken).Properties().ToList()[0].Name]! // Gets the ID of the entry
                ["type"]!.ToString(); // Gets the type of the entry
            
            // local method for constructing a SettingEntry
            void Construct()
            {
                AbstractEntry result = type switch
                {
                    "String" => new SettingEntry<string>(jToken),
                    "Int" => new SettingEntry<int>(jToken),
                    "Bool" => new SettingEntry<bool>(jToken),
                    _ => null
                };
                if (result is null) Assert.Fail("Test data might be broken!");
            }

            // Assert
            Assert.Throws<InvalidNameException>(Construct);
        }

        [Test,
         Description("Parse an entry without a type key. InvalidEntryTokenException is expected.")]
        public void Ctor_JsonEntryWithoutTypeKey_InvalidEntryTokenException() =>
            Assert.Throws<InvalidEntryTokenException>(() =>
                _ = new SettingEntry<string>(
                    JToken.Parse(
                        File.ReadAllText(TestData.SettingEntry.StringEntryWithoutTypeKeyJsonPath))));
        
        [Test,
         Description("Parse an int entry with string parameter. EntryTypeNotMatchException is expected.")]
        public void Ctor_TypeParameterAndJsonTypeKeyNotMatch_EntryTypeNotMatchException() =>
            Assert.Throws<EntryTypeNotMatchException>(() =>
                _ = new SettingEntry<string>(
                    JToken.Parse(
                        File.ReadAllText(TestData.SettingEntry.NormalIntEntryJsonPath))));

        [Test,
         Description("Parse a bool entry with string value. InvalidEntryValueException is expected.")]
        public void Ctor_ValueAndTypeNotMatch_InvalidEntryValueException() =>
            Assert.Throws<InvalidEntryValueException>(() =>
                _ = new SettingEntry<bool>(
                    JToken.Parse(
                        File.ReadAllText(TestData.SettingEntry.BoolEntryValueAndTypeNotMatchJsonPath))));

        [Test,
         Description("Parse a string entry without value key, InvalidEntryTokenException is expected.")]
        public void Ctor_JsonEntryWithoutValueKey_InvalidEntryTokenException() =>
            Assert.Throws<InvalidEntryTokenException>(() =>
                _ = new SettingEntry<string>(
                    JToken.Parse(
                        File.ReadAllText(TestData.SettingEntry.StringEntryWithoutValueKeyJsonPath))));

        [Test,
         Description("Gets the root of an orphan entry, null should be returned.")]
        public void Root_OrphanEntry_Null()
        {
            // Arrange
            var entry = new SettingEntry<string>("OrphanEntry", "...");
            
            // Act
            var root = entry.Root;
            
            // Assert
            Assert.That(root, Is.Null);
        }

        [Test,
         Description("Gets the root of an entry, the correct root group should be returned.")]
        public void Root_GetsTheCorrectRootOfTheCurrentEntry()
        {
            // Arrange
            var root = new SettingsGroup(JToken.Parse(File.ReadAllText(TestData.SettingEntry.NormalNestedEntry)), true);
            var entry = root["second-level-group"]["string-entry"] as SettingEntry<string>;
            
            // Act
            var rootOfEntry = entry!.Root;
            
            // Assert
            Assert.That(rootOfEntry, Is.Not.Null);
            Assert.That(rootOfEntry, Is.EqualTo(root));
        }
    }
}