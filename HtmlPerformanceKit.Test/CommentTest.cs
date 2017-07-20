using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlPerformanceKit.Test
{
    [TestClass]
    public class CommentTest
    {
        private readonly List<HtmlParseErrorEventArgs> parseErrors = new List<HtmlParseErrorEventArgs>();
        private HtmlReader reader;

        [TestMethod]
        public void EmptyBogusCommentState()
        {
            reader = HtmlReaderFactory.FromString("<!>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Comment, reader.TokenKind);
            Assert.AreEqual("", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(1, parseErrors.Count);
        }

        [TestMethod]
        public void EmptyBogusCommentState2()
        {
            reader = HtmlReaderFactory.FromString("<!", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Comment, reader.TokenKind);
            Assert.AreEqual("", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(1, parseErrors.Count);
        }

        [TestMethod]
        public void BogusCommentState()
        {
            reader = HtmlReaderFactory.FromString("<!div displayed>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Comment, reader.TokenKind);
            Assert.AreEqual("div displayed", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(1, parseErrors.Count);
        }

        [TestMethod]
        public void BogusCommentState2()
        {
            reader = HtmlReaderFactory.FromString("<!/div>", parseErrors);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlTokenKind.Comment, reader.TokenKind);
            Assert.AreEqual("/div", reader.Text);

            Assert.IsFalse(reader.Read());
            Assert.AreEqual(1, parseErrors.Count);
        }
    }
}
