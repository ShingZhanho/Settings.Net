#nullable enable
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

        [TestCase("SomeId", "Some description"),
         TestCase("SomeMoreId", null),
         Description("Add a new root to a bundle, no exceptions should be thrown.")]
        public void AddNewRoot_ConstructNewRoot_Successful(string id, string description)
        {
            // Arrange
            var bundle = new SettingsBundle();

            // Act
            bundle.AddNewRoot(id, null, description);
            
            // Assert
            Assert.That(bundle, Is.Not.Null);
            Assert.That(bundle.ContainsKey(id), Is.True);
            Assert.That(bundle[id], Is.Not.Null);
            Assert.That(bundle[id].IsRoot, Is.True);
        }

        [TestCase("SomeId", "Some description"),
         TestCase("someMoreIds", null)]
        public void AddNewRoot_ExistingRoot_Successful(string id, string description)
        {
            // Arrange
            var bundle = new SettingsBundle();
            var root = new SettingsGroup(id, null);

            // Act
            bundle.AddNewRoot(root);
            
            // Assert
            Assert.That(bundle.ContainsKey(id), Is.True);
            Assert.That(bundle[id], Is.Not.Null);
            Assert.That(bundle[id].IsRoot, Is.True);
        }
    }
}