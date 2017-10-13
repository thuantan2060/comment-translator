using System;
using System.Collections.Generic;
using System.Text;

namespace CommentTranslator.Parsers
{
    public class CSharpCommentParser : CommentParser
    {
        public CSharpCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //XML tags comment
                new ParseTag()
                {
                    Start = "///",
                    End = Environment.NewLine,
                    Name = "xmldoc"
                },

                //Singleline comment
                new ParseTag()
                {
                    Start = "//",
                    End = Environment.NewLine,
                    Name = "singleline"
                },

                //Multi line comment
                new ParseTag(){
                    Start = "/*",
                    End = "*/",
                    Name = "multiline"
                },

                //Singleline comment
                new ParseTag()
                {
                    Start = "//",
                    End = "",
                    Name = "singlelineend"
                },
            };
        }

        public override Comment GetComment(string commentText)
        {
            var lines = commentText.Split('\n');
            var builder = new StringBuilder();

            foreach(var line in lines)
            {
                var text = line.TrimStart();
                if (text.StartsWith("*"))
                {
                    text = text.Substring(1);
                }

                builder.AppendLine(text);
            }

            if (commentText.EndsWith(Environment.NewLine))
            {
                builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }

            return base.GetComment(builder.ToString());
        }

        public override TextPositions GetPositions(Comment comment)
        {
            if (comment.Origin.StartsWith("///"))
            {
                return TextPositions.Right;
            }

            return base.GetPositions(comment);
        }
    }
}
