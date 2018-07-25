

namespace Core.Helpers.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Extensions;

    [TestClass]
    public class StringHelperTests
    {
        [TestMethod]
        public void TestSafeSubstringSuccess()
        {
            string text = "This is a test string";
            string result = text.SafeSubstring(0, 10);
            Assert.AreEqual(result, "This is a ");
        }

        [TestMethod]
        public void TestSafeSubstringLengthTooLongSuccess()
        {
            string text = "This is a test string";
            string result = text.SafeSubstring(5, 200);
            Assert.AreEqual(result, "is a test string");
        }

        [TestMethod]
        public void TestSafeSubstringStartTooLargeSuccess()
        {
            string text = "This is a test string";
            string result = text.SafeSubstring(100, 5);
            Assert.AreEqual(result, string.Empty);
        }

        [TestMethod]
        public void TestSafeSubstringLengthTooLongStartTooLargeSuccess()
        {
            string text = "This is a test string";
            string result = text.SafeSubstring(100, 200);
            Assert.AreEqual(result, String.Empty);
        }

        [TestMethod]
        public void TestSafeSubstringNulleSuccess()
        {
            string text = null;
            string result = text.SafeSubstring(100, 200);
            Assert.AreEqual(result, String.Empty);
        }
    }
}
