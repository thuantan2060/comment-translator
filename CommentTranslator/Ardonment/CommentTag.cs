using CommentTranslator.Parsers;
using Microsoft.VisualStudio.Text.Tagging;

namespace CommentTranslator.Ardonment
{
    public class CommentTag : ITag
    {
        public string Text { get; set; }
        public int TimeWaitAfterChange { get; set; }
        public ICommentParser Parser { get; set; }
        public Comment Comment { get; set; }

        public CommentTag(string text, ICommentParser parser, int timeWaitAfterChange = 0)
        {
            Text = text;
            Parser = parser;
            TimeWaitAfterChange = timeWaitAfterChange;
            Comment = parser.GetComment(text);
        }
    }
}
