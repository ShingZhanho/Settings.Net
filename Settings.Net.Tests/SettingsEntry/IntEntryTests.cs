using System;
using System.Linq;
using NUnit.Framework;
using Settings.Exceptions;
using Settings.SettingsEntry;
using Newtonsoft.Json.Linq;

namespace Settings.Net.Tests.SettingsEntry {
    [TestFixture]
    public class IntEntryTests {
        [TestCase("IntEntry1", 123, "Description1"), TestCase("IntEntry2", 456, null),
         TestCase("IntEntry3", null, null)]
        public void ConstructNewEntry_ValidParameters_Successful(string id, int value, string desc) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out var result);
            result.Description = desc;
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result.ID, Is.EqualTo(id));
            Assert.That(result.Value, Is.EqualTo(value));
            Assert.That(result.ToString(), Is.EqualTo(value.ToString()));
            Assert.That(result.Description, Is.EqualTo(desc));
        }

        [TestCase(null, null), TestCase("", null)]
        public void ConstructNewEntry_NullId_ArgNullException(string id, int value) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(ArgumentNullException)));
            Console.WriteLine(exception.Message);
        }

        
        [TestCase("Invalid Int Entry", 777),
        TestCase("Another One", 689)]
        public void ConstructNewEntry_IllegalChars_InvalidNameException(string id, int value) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out var _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidNameException)));
            Console.WriteLine(exception.Message);
        }

        private static Exception GetExceptionFromConstructor(string json, out IntEntry result) {
            try {
                result = new IntEntry(JToken.Parse(json));
            } catch (Exception e) {
                result = null;
                return e;
            }
            // Constructor succeeded
            return null;
        }

        [TestCase(IntEntryJsonSource.NormalEntry)]
        public void ConstructorJsonEntry_ValidData_Successful(string json) {
            // Arrange
            var jToken = JToken.Parse(json);
            var entryId = ((JObject) jToken).Properties().ToList()[0].Name;
            var entryDesc = jToken[entryId]["desc"].ToString();
            var entryValue = int.Parse(jToken[entryId]["value"].ToString());
            
            // Act
            var exception = GetExceptionFromConstructor(json, out var result);
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result.ID, Is.EqualTo(entryId));
            Assert.That(result.Description, Is.EqualTo(entryDesc));
            Assert.That(result.Value, Is.EqualTo(entryValue));
            Assert.That(result.ToString(), Is.EqualTo(entryValue.ToString()));
        }

        [TestCase(IntEntryJsonSource.InvalidIdEntry1), TestCase(IntEntryJsonSource.InvalidIdEntry2)]
        public void ConstructJsonEntry_IllegalIdChars_InvalidNameException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
                        
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidNameException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(IntEntryJsonSource.InvalidType_StringType), TestCase(IntEntryJsonSource.InvalidType_BoolType)]
        public void ConstructJsonEntry_TypeNotMatch_EntryTypeNotMatchException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(EntryTypeNotMatchException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(IntEntryJsonSource.InvalidValueType_StringType),
         TestCase(IntEntryJsonSource.InvalidValueType_BoolType)]
        public void ConstructJsonEntry_ValueTypeNotMatch_InvalidEntryValueException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidEntryValueException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(IntEntryJsonSource.MissingTypeKey), TestCase(IntEntryJsonSource.MissingValueKey)]
        public void ConstructJsonEntry_MissingKeys_InvalidEntryTokenException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidEntryTokenException)));
            Console.WriteLine(exception.Message);
        }

        private static Exception GetExceptionFromConstructor(string id, int value, out IntEntry result) {
            try {
                result = new IntEntry(id, value);
            } catch (Exception e) {
                result = null;
                return e;
            }
            // Constructor succeeded
            return null;
        }
        
        private static class IntEntryJsonSource {
            // This class contains JSON data source for tests
            public const string NormalEntry = 
                @"{'EntryID':{'type':'Settings.IntEntry','desc':'A normal int entry','value':123}}";
            public const string InvalidIdEntry1 =
                @"{'Invalid.Entry':{'type':'Settings.IntEntry','desc':'Entry with invalid id name','value':123}}";
            public const string InvalidIdEntry2 =
                @"{'Invalid  Entry':{'type':'Settings.IntEntry','desc':'Entry with invalid id name','value':123}}";
            public const string InvalidType_StringType =
                @"{'StringEntry':{'type':'Settings.StringEntry','desc':'A string entry','value':'15'}}";
            public const string InvalidType_BoolType = 
                @"{'BoolEntry':{'type':'Settings.BoolEntry','desc':'An bool entry','value':true}}";
            public const string InvalidValueType_StringType =
                @"{'IntEntryWithStringValue':{'type':'Settings.IntEntry','desc':'An int entry with string value.','value':'15'}}";
            public const string InvalidValueType_BoolType =
                @"{'IntEntryWithBoolValue':{'type':'Settings.IntEntry','desc':'An int entry with bool value.','value':false}}";
            public const string MissingTypeKey =
                @"{'MissingTypeKey':{'desc':'An int entry with int value.','value':234}}";
            public const string MissingValueKey =
                @"{'MissingValueKey':{'type':'Settings.IntEntry','desc':'An int entry with int value.'}}";
        }
    }
}