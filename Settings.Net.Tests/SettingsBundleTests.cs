#nullable enable
using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

#pragma warning disable 8602

namespace Settings.Net.Tests
{
    public class SettingsBundleTests
    {
        [TestCase("description"),
         TestCase(null),
         Description("Construct a new SettingsBundle, no exceptions should be thrown.")]
        public void Ctor_NewBundle_Successful(string? description)
        {
            // Act
            var result = new SettingsBundle{Description = description};
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Description, Is.EqualTo(description));
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
         TestCase("someMoreIds", null),
         Description("Add an existing root to a bundle.")]
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

        [Test,
         Description("Adds a root with the same name of an existing root. InvalidOperationException is expected.")]
        public void AddNewRoot_ExistingRoot_InvalidOperationException()
        {
            // Arrange
            var bundle = new SettingsBundle();
            const string? id = "rootId";
            bundle.AddNewRoot(id);
            var exception = new Exception();
            
            // Act
            try
            {
                bundle.AddNewRoot(id);
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(InvalidOperationException)));
        }

        [Test,
         Description("Use indexer to get a root that does not exist. IndexOutOfRangeException is expected.")]
        public void Indexer_EntryNotExists_IndexOutOfRangeException()
        {
            // Arrange
            var bundle = new SettingsBundle();
            bundle.AddNewRoot("RealRoot");
            Exception exception = new();
            
            // Act
            try
            {
                _ = bundle["FakeRoot"];
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            // Assert
            Assert.That(exception, Is.TypeOf(typeof(IndexOutOfRangeException)));
        }
    }
}