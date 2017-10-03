using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class XamlCommentParser : CommentParser
    {
        public XamlCommentParser()
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
