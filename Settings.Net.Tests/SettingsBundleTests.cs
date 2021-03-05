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
            // Act
            var result = new SettingsBundle();
            
            // Assert
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
            var bundle = new SettingsBundle();

            // Act
            bundle.AddNewRoot(id, null, desc);
            
            // Assert
            Assert.That(bundle, Is.Not.Null);
            Assert.That(bundle.ContainsKey(id), Is.True);
        }
    }
}