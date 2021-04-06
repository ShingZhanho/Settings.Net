using System;
using System.Collections.Generic;
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
    }
}