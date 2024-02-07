using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class DataTest
    {
        private readonly List<HtmlParseErrorEventArgs> parseErrors = new List<HtmlParseErrorEventArgs>();
        private HtmlReader reader;

        [TestMethod]
        public void Empty()
        {
            reader = HtmlReaderFactory.FromString("", parseErrors);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void Data()
        {
            reader = HtmlReaderFactory.FromString("a", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("a", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataDecimalCharacterReference()
        {
            reader = HtmlReaderFactory.FromString("&#65;", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("A", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataDecimalCharacterReferenceWhenDecodingSkipped()
        {
            var options = new HtmlReaderOptions
            {
                DecodeHtmlCharacters = false
            };

            reader = HtmlReaderFactory.FromString("&#65;", parseErrors, options);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("&#65;", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataHexCharacterReference()
        {
            reader = HtmlReaderFactory.FromString("&#x41;", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("A", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataHexCharacterReferenceWhenDecodingSkipped()
        {
            var options = new HtmlReaderOptions
            {
                DecodeHtmlCharacters = false
            };

            reader = HtmlReaderFactory.FromString("&#x41;", parseErrors, options);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("&#x41;", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataNamedCharacterReference()
        {
            reader = HtmlReaderFactory.FromString("&lt;", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("<", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataNamedCharacterReferenceWhenDecodingSkipped()
        {
            var options = new HtmlReaderOptions
            {
                DecodeHtmlCharacters = false
            };

            reader = HtmlReaderFactory.FromString("&lt;", parseErrors, options);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("&lt;", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataNamedCharacterReference2()
        {
            reader = HtmlReaderFactory.FromString("I'm &notit; I tell you", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("I'm ¬it; I tell you", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(1, parseErrors.Count);
            Assert.AreEqual(1, parseErrors[0].LineNumber);
            Assert.AreEqual(6, parseErrors[0].LinePosition);
        }

        [TestMethod]
        public void DataNamedCharacterReference2WhenDecodingSkipped()
        {
            var options = new HtmlReaderOptions
            {
                DecodeHtmlCharacters = false
            };

            reader = HtmlReaderFactory.FromString("I'm &notit; I tell you", parseErrors, options);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("I'm &notit; I tell you", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataNamedCharacterReference3()
        {
            reader = HtmlReaderFactory.FromString("I'm &notin; I tell you", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("I'm ∉ I tell you", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }

        [TestMethod]
        public void DataNamedCharacterReference3WhenDecodingSkipped()
        {
            var options = new HtmlReaderOptions
            {
                DecodeHtmlCharacters = false
            };

            reader = HtmlReaderFactory.FromString("I'm &notin; I tell you", parseErrors, options);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Text, reader.TokenKind);
            Assert.AreEqual("I'm &notin; I tell you", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(0, parseErrors.Count);
        }
    }
}
