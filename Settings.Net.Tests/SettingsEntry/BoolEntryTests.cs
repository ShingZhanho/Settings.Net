using System;
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
    }
}