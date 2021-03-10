#nullable enable
using System;
using System.Collections.Generic;
using NUnit.Framework;

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
            var path = bundle.AddNewRoot(id, null, description);
            
            // Assert
            Assert.That(bundle, Is.Not.Null);
            Assert.That(bundle.ContainsKey(id), Is.True);
            Assert.That(bundle[id], Is.Not.Null);
            Assert.That(bundle[id].IsRoot, Is.True);
            Assert.That(path, Is.EqualTo(bundle[id].Path));
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
            var path = bundle.AddNewRoot(root);
            
            // Assert
            Assert.That(bundle.ContainsKey(id), Is.True);
            Assert.That(bundle[id], Is.Not.Null);
            Assert.That(bundle[id].IsRoot, Is.True);
            Assert.That(path, Is.EqualTo(bundle[id].Path));
        }

        [Test,
         Description("Adds a root with the same name of an existing root. InvalidOperationException is expected.")]
        public void AddNewRoot_ExistingRoot_InvalidOperationException()
        {
            // Arrange
            var bundle = new SettingsBundle();
            const string? id = "rootId";
            bundle.AddNewRoot(id);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => bundle.AddNewRoot(id));
        }

        [Test,
         Description("Use indexer to get a root that does not exist. IndexOutOfRangeException is expected.")]
        public void Indexer_EntryNotExists_IndexOutOfRangeException()
        {
            // Arrange
            var bundle = new SettingsBundle();
            bundle.AddNewRoot("RealRoot");
            
            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => _ = bundle["FakeRoot"]);
        }

        [Test,
         Description("Remove an existing empty bundle from a bundle.")]
        public void RemoveRoot_RemoveExistingEmptyRoot_EntryRemoved()
        {
            // Arrange
            var bundle = new SettingsBundle();
            bundle.AddNewRoot("rootToRemove");
            
            // Act
            bundle.RemoveRoot("rootToRemove");
            
            // Assert
            Assert.That(bundle.ContainsKey("rootToRemove"), Is.False);
        }

        [Test,
         Description("Remove an root that has children items without setting 'recursive' to true." +
                     "InvalidOperationException should be thrown.")]
        public void RemoveRoot_RemoveNonEmptyRootWithoutRecursive_InvalidOperationException()
        {
            // Arrange
            var bundle = new SettingsBundle();
            bundle.AddNewRoot(new SettingsGroup(
                "GroupWithChildren",
                new List<IEntry>
                {
                    new SettingEntry<string>("StringEntry", "Some value")
                }));
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => bundle.RemoveRoot("GroupWithChildren"));
        }

        [Test,
         Description("Remove an root that does not exist in the bundle. ArgumentOutOfRangeException should be thrown.")]
        public void RemoveRoot_RemoveNonExistingRoot_ArgumentOutOfRangeException()
        {
            // Arrange
            var bundle = new SettingsBundle();
            bundle.AddNewRoot("This-Is-A-Root");
            
            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => bundle.RemoveRoot("This-Is-Not-A-Root"));
        }

        [Test]
        public void GetEntryByPath_ValidPath_CorrespondingEntry()
        {
            // Arrange
            var expectedEntry = GetEntryByPathTestBundle["Root1"]["Group1"]["G1-String"];
            
            // Act
            var actualEntry = GetEntryByPathTestBundle.GetEntryByPath("Root1.Group1.G1-String");
            
            // Assert
            Assert.That(actualEntry, Is.Not.Null);
            Assert.That(actualEntry.Path, Is.EqualTo("Root1.Group1.G1-String"));
        }

        private static SettingsBundle GetEntryByPathTestBundle 
        {
            get
            {
                var bundle = new SettingsBundle();
                bundle.AddNewRoot(
                    new SettingsGroup("Root1",
                        new List<IEntry>
                        {
                            new SettingsGroup(
                                "Group1",
                                new List<IEntry>
                                {
                                    new SettingEntry<string>("G1-String", "Values")
                                }),
                            new SettingEntry<int>("R1-Int", 123)
                        }));
                return bundle;
            }
        }
    }
}