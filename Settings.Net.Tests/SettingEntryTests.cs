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
    }
}