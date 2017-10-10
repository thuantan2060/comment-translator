using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public enum TextPositions
    {
        Bottom,
        Right
    }

    public interface ICommentParser
    {
        IEnumerable<Comment> GetComments(SnapshotSpan span);
        Comment GetComment(string comment);
        string TrimComment(string comment);
        string SimpleTrimComment(string comment);
        TextPositions GetPositions(string comment);
    }
}
