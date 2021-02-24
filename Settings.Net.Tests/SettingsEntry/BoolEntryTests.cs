using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Settings.Exceptions;
using Settings.SettingsEntry;

namespace Settings.Tests.SettingsEntry {
    [TestFixture]
    public class BoolEntryTests {
        [TestCase("BoolEntry1", false, "Description1"), TestCase("BoolEntry2", true, null),
         TestCase("BoolEntry3", null, null)]
        public void ConstructNewEntry_ValidParameters_Successful(string id, bool? value, string desc) {
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

        [TestCase(null, null), TestCase(null, false)]
        public void ConstructNewEntry_NullId_ArgNullException(string id, bool? value) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(ArgumentNullException)));
            Console.WriteLine(exception.Message);
        }

        [TestCase("Invalid Bool Entry", true),
        TestCase("Invalid.Bool.Entry", false)]
        public void ConstructNewEntry_IllegalChars_InvalidNameException(string id, bool? value) {
            // Act
            var exception = GetExceptionFromConstructor(id, value, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidNameException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(BoolEntryJsonSource.NormalEntry)]
        public void ConstructorJsonEntry_ValidData_Successful(string json) {
            // Arrange
            var jToken = JToken.Parse(json);
            var entryId = ((JObject) jToken).Properties().ToList()[0].Name;
            var entryDesc = jToken[entryId]["desc"].ToString();
            var entryValue = (bool)jToken[entryId]["value"];
            
            // Act
            var exception = GetExceptionFromConstructor(json, out var result);
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result.ID, Is.EqualTo(entryId));
            Assert.That(result.Description, Is.EqualTo(entryDesc));
            Assert.That(result.Value, Is.EqualTo(entryValue));
            Assert.That(result.ToString(), Is.EqualTo(entryValue.ToString()));
        }

        [TestCase(BoolEntryJsonSource.InvalidIdEntry1), TestCase(BoolEntryJsonSource.InvalidIdEntry2)]
        public void ConstructJsonEntry_IllegalIdChars_InvalidNameException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
                        
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidNameException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(BoolEntryJsonSource.InvalidType_StringType), TestCase(BoolEntryJsonSource.InvalidType_IntType)]
        public void ConstructJsonEntry_TypeNotMatch_EntryTypeNotMatchException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(EntryTypeNotMatchException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(BoolEntryJsonSource.InvalidValueType_StringType),
         TestCase(BoolEntryJsonSource.InvalidValueType_IntType)]
        public void ConstructJsonEntry_ValueTypeNotMatch_InvalidEntryValueException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidEntryValueException)));
            Console.WriteLine(exception.Message);
        }
        
        [TestCase(BoolEntryJsonSource.MissingTypeKey), TestCase(BoolEntryJsonSource.MissingValueKey)]
        public void ConstructJsonEntry_MissingKeys_InvalidEntryTokenException(string json) {
            // Act
            var exception = GetExceptionFromConstructor(json, out _);
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidEntryTokenException)));
            Console.WriteLine(exception.Message);
        }

        private static Exception GetExceptionFromConstructor(string json, out BoolEntry result) {
            try {
                result = new BoolEntry(JToken.Parse(json));
            } catch (Exception e) {
                result = null;
                return e;
            }
            // Constructor succeeded
            return null;
        }

        private static Exception GetExceptionFromConstructor(string id, bool? value, out BoolEntry result) {
            try {
                result = new BoolEntry(id, value);
            } catch (Exception e) {
                result = null;
                return e;
            }
            // Constructor succeeded
            return null;
        }
        
        private static class BoolEntryJsonSource {
            // This class contains JSON data source for tests
            public const string NormalEntry =
                @"{'EntryID':{'type':'Settings.BoolEntry','desc':'normal bool','value':true}}";
            public const string NullValueEntry =
                @"{'NullBool':{'type':'Settings.BoolEntry','desc':'null bool','value':null}}";
            public const string InvalidIdEntry1 =
                @"{'Invalid.Entry':{'type':'Settings.BoolEntry','desc':'invalid name','value':false}}";
            public const string InvalidIdEntry2 =
                @"{'Invalid Entry Name':{'type':'Settings.BoolEntry','desc':'invalid name','value':false}}";
            public const string InvalidType_StringType =
                @"{'stringEntry':{'type':'Settings.StringEntry','desc':'string with bool value','value':false}}";
            public const string InvalidType_IntType =
                @"{'intEntry':{'type':'Settings.IntEntry','desc':'int with bool value','value':false}}";
            public const string InvalidValueType_StringType =
                @"{'StringEntryWithBoolValue':{'type':'Settings.BoolEntry','desc':'A bool entry with string value.','value':'false'}}";
            public const string InvalidValueType_IntType =
                @"{'IntEntryWithIntValue':{'type':'Settings.BoolEntry','desc':'A bool entry with int value.','value':15}}";
            public const string MissingTypeKey =
                @"{'MissingTypeKey':{'desc':'A bool entry without type key.','value':true}}";
            public const string MissingValueKey =
                @"{'MissingValueKey':{'type':'Settings.BoolEntry','desc':'A bool entry without value key.'}}";
        }
    }
}