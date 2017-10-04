using CommentTranslator.Util;
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
                    End = "",
                    Name = "xmldoc"
                },

                //Singleline comment
                new CommentTag()
                {
                    Start = "//",
                    End = "",
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

        public override TrimComment TrimComment(string comment)
        {
            foreach (var tag in Tags)
            {
                var startIndex = comment.IndexOf(tag.Start);
                if (startIndex >= 0)
                {
                    //Shif start index
                    startIndex += tag.Start.Length;

                    //Calculate end index
                    var endIndex = tag.End.Length == 0 ? comment.Length : comment.IndexOf(tag.End);
                    if (startIndex >= endIndex)
                    {
                        return new TrimComment()
                        {
                            OriginText = comment,
                            LineCount = 0,
                            TrimedText = ""
                        };
                    }

                    //Break into lines
                    var text = comment.Substring(startIndex, endIndex - startIndex);
                    var lines = text.Split('\n');

                    //Check if single line
                    if (lines.Length <= 1)
                    {
                        return new TrimComment()
                        {
                            OriginText = comment,
                            LineCount = 1,
                            TrimedText = text.Trim()
                        };
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

                    var trimedText = builder.ToString().TrimEnd();

                    return new TrimComment()
                    {
                        OriginText = comment,
                        LineCount = CommentHelper.LineCount(trimedText),
                        TrimedText = trimedText
                    };
                }
            }

            return new TrimComment()
            {
                OriginText = comment,
                LineCount = CommentHelper.LineCount(comment),
                TrimedText = comment
            };
        }
    }
}
