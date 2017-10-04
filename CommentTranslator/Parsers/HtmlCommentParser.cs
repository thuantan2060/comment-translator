using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class HtmlCommentParser : CommentParser
    {
        public HtmlCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //Multi line comment
                new CommentTag(){
                    Start = "<!--",
                    End = "-->",
                    Name = "multiline"
                }
            };
        }
    }
}
