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
        IEnumerable<CommentRegion> GetCommentRegions(ITextSnapshot snapshot);
        //Comment GetComment(Ardonment.CommentTag comment);
        TrimmedText TrimComment(string comment);
        string TrimCommentLines(string comment);
        TextPositions GetPositions(Ardonment.CommentTag comment);
        //bool IsValidComment(string comment);
    }

    public class CommentRegion
    {
        public int Start { get; set; }
        public int Length { get; set; }
    }
}
