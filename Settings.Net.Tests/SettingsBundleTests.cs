#nullable enable
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

#pragma warning disable 8602

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

        [Test,
        Description("Add a new root to a bundle, no exceptions should be thrown.")]
        public void AddNewRoot_ConstructNewRoot_Successful()
        {
            // Arrange
            var randomizer = new Randomizer();
            var id = randomizer.GetString(randomizer.Next(1,15));
            var desc = randomizer.GetString(randomizer.Next(5, 30));
            static SettingsBundle Action(object? parameter)
            {
                var list = (List<string>) parameter!;
                var bundle = new SettingsBundle();
                bundle.AddNewRoot(list[0], null, list[1]);
                return bundle;
            }

            // Act
            var exception = TestUtils.GetExceptionFromFunction(Action, new List<string> {id, desc}, out var result);
            
            // Assert
            Assert.That(exception, Is.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey(id), Is.True);
        }
    }
}