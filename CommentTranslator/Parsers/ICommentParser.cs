using CommentTranslator.Ardonment;
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
        Comment GetComment(CommentTranslateTag comment);
        TrimmedText TrimComment(string comment);
        string SimpleTrimComment(string comment);
        TextPositions GetPositions(CommentTranslateTag comment);
        bool IsValidComment(string comment);
    }
}
