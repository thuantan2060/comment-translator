using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class CssCommentParser : CommentParser
    {
        public CssCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Multi line comment
                new ParseTag(){
                    Start = "/*",
                    End = "*/",
                    Name = "comment"
                }
            };
        }
    }
}
