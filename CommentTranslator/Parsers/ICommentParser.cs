using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public interface ICommentParser
    {
        IEnumerable<Comment> GetComment(SnapshotSpan span);
    }
}
