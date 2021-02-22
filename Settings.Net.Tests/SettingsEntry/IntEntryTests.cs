using System;
using System.Linq;
using NUnit.Framework;
using Settings.Net.Exceptions;
using Settings.Net.SettingsEntry;
using Newtonsoft.Json.Linq;

namespace Settings.Net.Tests.SettingsEntry {
    [TestFixture]
    public class IntEntryTests {
        [TestCase("IntEntry1", 123, "Description1")]
        [TestCase("IntEntry2", 456, null)]
        [TestCase("IntEntry3", null, null)]
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
    }
}