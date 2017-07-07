using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlSpeedPack.Test
{
    [TestClass]
    public class HtmlReaderTest
    {
        [TestMethod]
        public void Empty()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("")));
            var reader = new HtmlReader(stream);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void Data()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("a")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("a", reader.Text);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void Element()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<br/>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("br", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementData()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<br/>a")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("br", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("a", reader.Text);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void DataElement()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("a<br/>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("a", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("br", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void DataElementDataElement()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("a<br/>b<p/>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("a", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("br", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("b", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("p", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementDataElementData()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<br/>a<p/>b")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("br", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("a", reader.Text);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("p", reader.Name);
            Assert.IsTrue(reader.SelfClosingElement);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("b", reader.Text);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementDoubleQuotedAttributeValue()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a href=\"javascript:;\">")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("a", reader.Name);
            Assert.AreEqual("javascript:;", reader.GetAttribute("href"));
            Assert.IsFalse(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementSingleQuotedAttributeValue()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a href='javascript:;'>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("a", reader.Name);
            Assert.AreEqual("javascript:;", reader.GetAttribute("href"));
            Assert.IsFalse(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementUnquotedAttributeValue()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a href=javascript:;>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("a", reader.Name);
            Assert.AreEqual("javascript:;", reader.GetAttribute("href"));
            Assert.IsFalse(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementNoAttributeValue()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a href>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("a", reader.Name);
            Assert.AreEqual("", reader.GetAttribute("href"));
            Assert.IsFalse(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void ElementMissingAttribute()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Tag, reader.NodeType);
            Assert.AreEqual("a", reader.Name);
            Assert.IsNull(reader.GetAttribute("href"));
            Assert.IsFalse(reader.SelfClosingElement);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void EmptyBogusCommentState()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<!>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Comment, reader.NodeType);
            Assert.AreEqual("", reader.Comment);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void EmptyBogusCommentState2()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<!")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Comment, reader.NodeType);
            Assert.AreEqual("", reader.Comment);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void BogusCommentState()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<!div displayed>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Comment, reader.NodeType);
            Assert.AreEqual("div displayed", reader.Comment);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void BogusCommentState2()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<!/div>")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Comment, reader.NodeType);
            Assert.AreEqual("/div", reader.Comment);

            Assert.IsFalse(reader.Read());
        }


        [TestMethod]
        public void DataDecimalCharacterReference()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("&#65;")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("A", reader.Text);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void DataHexCharacterReference()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("&#x41;")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("A", reader.Text);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void DataNamedCharacterReference()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("&lt;")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(HtmlNodeType.Text, reader.NodeType);
            Assert.AreEqual("<", reader.Text);

            Assert.IsFalse(reader.Read());
        }


        [TestMethod]
        public void AttributeValueDecimalCharacterReference()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a title=\"&#65;\">")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual("A", reader.GetAttribute("title"));

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void AttributeValueHexCharacterReference()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a title=\"&#x41;\">")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual("A", reader.GetAttribute("title"));

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void AttributeValueNamedCharacterReference()
        {
            var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("<a title=\"&lt;\">")));
            var reader = new HtmlReader(stream);

            Assert.IsTrue(reader.Read());
            Assert.AreEqual("<", reader.GetAttribute("title"));

            Assert.IsFalse(reader.Read());
        }
    }
}
