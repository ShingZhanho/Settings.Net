﻿#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Settings.Net.Exceptions;

#pragma warning disable 8602

namespace Settings.Net.Tests
{
    [TestFixture]
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

        [Test,
         Description("Construct a SettingBundle with a file.")]
        public void Ctor_SettingsFilePath_Successful()
        {
            // Act
            var result = new SettingsBundle(TestData.SettingsBundle.NormalJsonFilePath);
            
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test,
         Description("Construct a SettingsBundle with invalid JSON data. JsonReaderException should be thrown.")]
        public void Ctor_InvalidJsonString_JsonReaderException() =>
            Assert.Throws<JsonReaderException>(
                () => _ = new SettingsBundle(TestData.SettingsBundle.InvalidJsonFilePath));

        [Test,
         Description("Construct a SettingsBundle with empty file path. Expected ArgumentException.")]
        public void Ctor_EmptyPath_ArgumentException() => Assert.Throws<ArgumentException>(() => _ = new SettingsBundle(string.Empty));

        [Test,
         Description("Construct a SettingsBundle with path that points to a file that is not exist. " +
                     "FileNotFoundException is expected.")]
        public void Ctor_NonExistingPath_FileNotFoundException() =>
            Assert.Throws<FileNotFoundException>(() => _ = new SettingsBundle("This/path/does/not/exist///"));

        [Test,
         Description("Construct from JSON that does not contain a 'metadata' key.")]
        public void Ctor_JsonWithoutMetadata_InvalidEntryTokenException() =>
            Assert.Throws<InvalidEntryTokenException>(() =>
                _ = new SettingsBundle(TestData.SettingsBundle.JsonWithoutMetadataFilePath));
        
        [Test,
         Description("Construct from JSON that does not contain a 'data' key.")]
        public void Ctor_JsonWithoutData_InvalidEntryTokenException() =>
            Assert.Throws<InvalidEntryTokenException>(() =>
                _ = new SettingsBundle(TestData.SettingsBundle.JsonWithoutDataFilePath));

        [Test,
         Description("Construct from valid JToken.")]
        public void Ctor_NormalJToken_Successful()
        {
            // Arrange
            var json = File.ReadAllText(TestData.SettingsBundle.NormalJsonFilePath);
            var token = JToken.Parse(json);
            
            // Act
            var result = new SettingsBundle(token);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Description, Is.Not.Null);
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
                new List<AbstractEntry>
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

        [Test,
         Description("Gets an entry by its path.")]
        public void GetEntryByPath_ValidPath_CorrespondingEntry()
        {
            // Arrange
            var expectedEntry = GetEntryByPathTestBundle["Root1"]["Group1"]["G1-String"];
            
            // Act
            var actualEntry = GetEntryByPathTestBundle.GetEntryByPath("Root1.Group1.G1-String");
            
            // Assert
            Assert.That(actualEntry, Is.Not.Null);
            Assert.That(actualEntry.Path, Is.EqualTo(expectedEntry.Path));
            Assert.That(actualEntry.ToString(), Is.EqualTo(expectedEntry.ToString()));
        }

        [Test,
         Description("Gets an entry by path. The middle of the path points to an entry. InvalidOperationException is expected.")]
        public void GetEntryByPath_PartOfPathInvalid_InvalidOperationException()
        {
            // Arrange
            const string? invalidPath = "Root1.R1-Int.G1-String";
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => GetEntryByPathTestBundle.GetEntryByPath(invalidPath));
        }

        [Test,
         Description("Gets an entry by path. The path points to an entry that does not exist. Null should be returned")]
        public void GetEntryByPath_EntryNotExisted_Null()
        {
            // Arrange
            const string? invalidPath = "Root1.ThisIsNotAPath";
            
            // Act
            var actualResult = GetEntryByPathTestBundle.GetEntryByPath(invalidPath);
            
            // Assert
            Assert.That(actualResult, Is.Null);
        }

        private static SettingsBundle GetEntryByPathTestBundle 
        {
            get
            {
                var bundle = new SettingsBundle();
                bundle.AddNewRoot(
                    new SettingsGroup("Root1",
                        new List<AbstractEntry>
                        {
                            new SettingsGroup(
                                "Group1",
                                new List<AbstractEntry>
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