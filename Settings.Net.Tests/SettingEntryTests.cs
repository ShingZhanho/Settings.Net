using System;
using System.Collections.Generic;
using NUnit.Framework;
using Settings.Net.Exceptions;

namespace Settings.Net.Tests
{
    public class SettingEntryTests
    {
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