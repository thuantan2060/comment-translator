using CommentTranslator.Ardonment;
using System.Collections.Generic;

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
                    Start = "//",
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

        public override Comment GetComment(CommentTranslateTag comment)
        {
            if (comment.Text.StartsWith("/*") && comment.Text.EndsWith("")) comment.Text += "*/";
            if (!comment.Text.StartsWith("/*") && !comment.Text.EndsWith("*/") && !comment.Text.StartsWith("//")) comment.Text = "//" + comment.Text;
            if (comment.Text.StartsWith("/*") && comment.Text.EndsWith("*/")) base.GetComment(comment);

           

            return base.GetComment(comment);
        }
    }
}
