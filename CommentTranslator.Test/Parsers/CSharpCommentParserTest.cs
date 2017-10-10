using CommentTranslator.Parsers;
using Microsoft.VisualStudio.Text;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace CommentTranslator.Test.Parsers
{
    [TestFixture]
    public class CSharpCommentParserTest
    {
        [TestCase("//This is single line comment\r\n", 1)]
        [TestCase("/*This is multi line comment 1\r\nThis is multi line comment 2*/", 1)]
        [TestCase("//This is single line comment 1\r\n//This is single line comment 2\r\n", 2)]
        [TestCase("/*comment 1*/\r\n/*comment 2*/\r\n/*comment 3*/\r\n", 3)]
        [TestCase("/*comment 1*//*comment 2*//*comment 3*/", 3)]
        [TestCase("/*comment //this is single line comment*/", 1)]
        [TestCase("//This is single /*comment*/ line comment\r\n", 1)]
        public void GetCommentTest(string text, int commentExpect)
        {
            var textSnapShot = new Mock<ITextSnapshot>();
            textSnapShot.Setup(t => t.GetText(It.IsAny<Span>())).Returns(text);

            var parser = new CSharpCommentParser();
            var comments = parser.GetComments(new SnapshotSpan(textSnapShot.Object, new Span()));

            Assert.AreEqual(commentExpect, comments.Count());
        }
    }
}
