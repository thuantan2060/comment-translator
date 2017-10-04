using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class CppCommentParser : CommentParser
    {
        public CppCommentParser()
        {
            Tags = new List<CommentTag>
            {
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
    }
}
