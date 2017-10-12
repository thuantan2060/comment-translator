using System;
using System.Collections.Generic;
using System.Text;

namespace CommentTranslator.Parsers
{
    public class CSharpCommentParser : CommentParser
    {
        public CSharpCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //XML tags comment
                new CommentTag()
                {
                    Start = "///",
                    End = "\n",
                    Name = "xmldoc"
                },

                //Singleline comment
                new CommentTag()
                {
                    Start = "//",
                    End = "\n",
                    Name = "singleline"
                },

                //Multi line comment
                new CommentTag(){
                    Start = "/*",
                    End = "*/",
                    Name = "multiline"
                }
            };
        }

        public override TrimmedText TrimComment(string comment)
        {
            foreach (var tag in Tags)
            {
                var startIndex = comment.IndexOf(tag.Start);
                if (startIndex >= 0)
                {
                    //Shiff start index
                    startIndex += tag.Start.Length;

                    //Calculate end index
                    var endIndex = 0;
                    if (tag.End.Length == 0)
                        endIndex = comment.EndsWith("\n") ? comment.Length - 1 : comment.Length;
                    else
                        endIndex = comment.IndexOf(tag.End);

                    if (startIndex >= endIndex)
                    {
                        return new TrimmedText("");
                    }

                    //Break into lines
                    var text = comment.Substring(startIndex, endIndex - startIndex);
                    var lines = text.Split('\n');

                    //Check if single line
                    if (lines.Length <= 1)
                    {
                        return new TrimmedText(text.Trim(), 1, 0);
                    }

                    //Trim multi line comment
                    var builder = new StringBuilder();

                    //Add first line
                    builder.AppendLine(lines[0].Trim());

                    //Add next lines
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim();

                        //Trim * charater
                        if (line.StartsWith("*"))
                        {
                            line = line.Substring(1).TrimStart();
                        }

                        builder.AppendLine(line);
                    }

                    var trimmedComment = builder.ToString().TrimEnd();
                    return new TrimmedText(trimmedComment, lines.Length, MarginTop(trimmedComment));
                }
            }

            return new TrimmedText(comment);
        }

        public override TextPositions GetPositions(Ardonment.CommentTag comment)
        {
            if (comment.ClassificationType?.Classification.IndexOf("doc", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return TextPositions.Right;
            }

            return base.GetPositions(comment);
        }
    }
}
