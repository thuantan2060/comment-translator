using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class RazorCommentParser : CommentParser
    {
        public RazorCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Multi line comment
                new ParseTag(){
                    Start = "@*",
                    End = "*@",
                    Name = "multiline2"
                }
            };
        }
    }
}
