using System.Collections.Generic;
using System.Linq;
using CommentTranslator.Ardonment;

namespace CommentTranslator.Parsers
{
    public class JavaScriptCommentParser : CommentParser
    {
        List<CommentTag> _trimTags;
        List<CommentTag> _removeTags;

        public JavaScriptCommentParser()
        {
            Tags = new List<CommentTag>
            {
                new CommentTag()
                {
                    Start = "/*",
                    End = "*/",
                    Name = "multiline"
                },
                new CommentTag()
                {
                    Start = "",
                    End = "",
                    Name = "comment"
                },
            };

            _trimTags = new List<CommentTag>()
            {
                new CommentTag()
                {
                    Start = "/*",
                    End = "*/"
                },
                new CommentTag()
                {
                    Start = "//",
                    End = ""
                },
            };

            _removeTags = new List<CommentTag>()
            {
                new CommentTag()
                {
                    Start = "",
                    End = "*/"
                },
            };
        }

        public override TrimmedText TrimComment(string comment)
        {
            foreach (var tag in _trimTags)
            {
                if (comment == tag.Start || comment == tag.End)
                {
                    return new TrimmedText("");
                }
            }

            if (comment.StartsWith("/*") && comment.EndsWith("*/")) return base.TrimComment(comment);

            foreach (var tag in _removeTags)
            {
                if (comment.StartsWith(tag.Start) && comment.EndsWith(tag.End))
                {
                    return new TrimmedText("");
                }
            }

            return base.TrimComment(comment);
        }

        public override Comment GetComment(CommentTranslateTag comment)
        {
            if (comment.Text.StartsWith("/*") && comment.Text.EndsWith("")) comment.Text+="*/";
            return base.GetComment(comment);
        }
    }
}
