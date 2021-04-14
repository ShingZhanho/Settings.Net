using NUnit.Framework;
using Settings.Net.Exceptions;

namespace Settings.Net.Tests
{
    [TestFixture]
    public class SettingsGroupTests
    {
        [TestCase("invalid.id"),
         TestCase("invalid id 2"),
         Description("Pass an invalid ID to the constructor. An InvalidNameException is expected.")]
        public void Ctor_InvalidId_InvalidNameException(string id) =>
            Assert.Throws<InvalidNameException>(() =>
                _ = new SettingsGroup(id, null, false));
    }
}