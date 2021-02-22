using System;
using System.Linq;
using NUnit.Framework;
using Settings.Net.Exceptions;
using Settings.Net.SettingsEntry;
using Newtonsoft.Json.Linq;

namespace Settings.Net.Tests.SettingsEntry {
    [TestFixture]
    public class StringEntryTests {
        [TestCase("StringEntry1", "Value1", "Description1")]
        [TestCase("StringEntry2", "Value2", null)]
        [TestCase("StringEntry3", null, null)]
        public void ConstructNewEntry_ValidParameters_Successful(string id, string value, string desc) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out var result);
            result.Description = desc;
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result.ID, Is.EqualTo(id));
            Assert.That(result.Value, Is.EqualTo(value));
            Assert.That(result.ToString(), Is.EqualTo(value));
            Assert.That(result.Description, Is.EqualTo(desc));
        }

        [TestCase(null, null), TestCase("", null)]
        public void ConstructNewEntry_NullId_ArgNullException(string id, string value) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(ArgumentNullException)));
            Console.WriteLine(exception.Message);
        }

        [TestCase("Invalid.Name", "some value")]
        [TestCase("Another Invalid Name", "some value")]
        public void ConstructNewEntry_IllegalChars_InvalidNameException(string id, string value) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidNameException)));
            Console.WriteLine(exception.Message);
        }

        [TestCase(StringEntryJsonSource.NormalEntry)]
        public void ConstructJsonEntry_ValidData_Successful(string json) {
            // Arrange
            var jToken = JToken.Parse(json);
            var entryID = ((JObject) jToken).Properties().ToList()[0].Name;
            var entryDesc = jToken[entryID]["desc"]?.ToString();
            var entryValue = jToken[entryID]["value"]?.ToString();
            
            // Act
            var exception = GetExceptionFromConstructor(json, out var result);
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result.ID, Is.EqualTo(entryID));
            Assert.That(result.Description, Is.EqualTo(entryDesc));
            Assert.That(result.Value, Is.EqualTo(entryValue));
            Assert.That(result.ToString(), Is.EqualTo(entryValue));
        }

        [TestCase(StringEntryJsonSource.InvalidIdEntry1)]
        [TestCase(StringEntryJsonSource.InvalidIdEntry2)]
        public void ConstructJsonEntry_IllegalIdChars_InvalidNameException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidNameException)));
            Console.WriteLine(exception.Message);
        }

        [TestCase(StringEntryJsonSource.InvalidType_IntType)]
        [TestCase(StringEntryJsonSource.InvalidType_BoolType)]
        public void ConstructJsonEntry_TypeNotMatch_EntryTypeNotMatchException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(EntryTypeNotMatchException)));
            Console.WriteLine(exception.Message);
        }

        [TestCase(StringEntryJsonSource.InvalidValueType_IntType)]
        [TestCase(StringEntryJsonSource.InvalidValueType_BoolType)]
        public void ConstructJsonEntry_ValueTypeNotMatch_InvalidEntryValueException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidEntryValueException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(StringEntryJsonSource.MissingTypeKey)]
        [TestCase(StringEntryJsonSource.MissingValueKey)]
        public void ConstructJsonEntry_MissingKeys_InvalidEntryTokenException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidEntryTokenException)));
            Console.WriteLine(exception.Message);
        }

        private static Exception GetExceptionFromConstructor(string json, out StringEntry result) {
            try {
                result = new StringEntry(JToken.Parse(json));
            } catch (Exception e) {
                result = null;
                return e;
            }
            // Constructor succeeded
            return null;
        }
        
        private static Exception GetExceptionFromConstructor(string id, string value, out StringEntry result) {
            try {
                result = new StringEntry(id, value);
            } catch (Exception e) {
                result = null;
                return e;
            }
            // Constructor succeeded
            return null;
        }

        private static class StringEntryJsonSource {
            // This class contains JSON data source for tests
            public const string NormalEntry = 
                @"{'EntryID':{'type':'Settings.StringEntry','desc':'A normal string entry','value':'value'}}";
            public const string InvalidIdEntry1 =
                @"{'Invalid.Entry':{'type':'Settings.StringEntry','desc':'Entry with invalid id name','value':'something'}}";
            public const string InvalidIdEntry2 =
                @"{'Invalid  Entry':{'type':'Settings.StringEntry','desc':'Entry with invalid id name','value':'something'}}";
            public const string InvalidType_IntType =
                @"{'IntEntry':{'type':'Settings.IntEntry','desc':'An int entry','value':15}}";
            public const string InvalidType_BoolType = 
                @"{'BoolEntry':{'type':'Settings.BoolEntry','desc':'An bool entry','value':true}}";
            public const string InvalidValueType_IntType =
                @"{'StringEntryWithIntValue':{'type':'Settings.StringEntry','desc':'A string entry with int value.','value':15}}";
            public const string InvalidValueType_BoolType =
                @"{'StringEntryWithBoolValue':{'type':'Settings.StringEntry','desc':'A string entry with bool value.','value':false}}";
            public const string MissingTypeKey =
                @"{'MissingTypeKey':{'desc':'A string entry with int value.','value':'something'}}";
            public const string MissingValueKey =
                @"{'MissingTypeKey':{'type':'Settings.StringEntry','desc':'A string entry with int value.'}}";
        }
    }
}