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
        IEnumerable<CommentRegion> GetCommentRegions(ITextSnapshot snapshot, int startFrom = 0);
        IEnumerable<CommentRegion> GetCommentRegions(string text, int startFrom = 0);
        Comment GetComment(string commentText);
        TextPositions GetPositions(Comment comment);
    }

    public class CommentRegion
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public int End { get { return Start + Length; } }
    }
}
