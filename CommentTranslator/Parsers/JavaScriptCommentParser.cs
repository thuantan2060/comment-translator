using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class JavaScriptCommentParser : CommentParser
    {
        public JavaScriptCommentParser()
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
                },

                //Default comment
                new CommentTag()
                {
                    Start = "",
                    End = "",
                    Name = "default"
                }
            };
        }
    }
}
