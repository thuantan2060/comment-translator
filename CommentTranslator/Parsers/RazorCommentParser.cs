using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class RazorCommentParser : CommentParser
    {
        public RazorCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //Multi line comment
                new CommentTag(){
                    Start = "@*",
                    End = "*@",
                    Name = "multiline2"
                }
            };
        }
    }
}
