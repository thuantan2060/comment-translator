using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class CssCommentParser : CommentParser
    {
        public CssCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //Multi line comment
                new CommentTag(){
                    Start = "/*",
                    End = "*/",
                    Name = "comment"
                }
            };
        }
    }
}
