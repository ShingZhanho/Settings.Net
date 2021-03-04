#nullable enable
using System;
using NUnit.Framework;

namespace Settings.Net.Tests
{
    public class SettingsBundleTests
    {
        [Test,
        Description("Construct a new SettingsBundle, no exceptions should be thrown.")]
        public void Ctor_NewBundle_Successful()
        {
            // Arrange
            static SettingsBundle Action(object? _) => new ();

            // Act
            var exception = TestUtils.GetExceptionFromFunction(Action, null, out var result);
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Roots.Count, Is.EqualTo(0));
        }
    }
}