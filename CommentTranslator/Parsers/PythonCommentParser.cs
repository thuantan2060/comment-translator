using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class PythonCommentParser : CommentParser
    {
        public PythonCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //Singleline comment
                new CommentTag()
                {
                    Start = "#",
                    End = "",
                    Name = "singleline"
                },

                //Multi line comment
                new CommentTag(){
                    Start = "'''",
                    End = "'''",
                    Name = "multiline"
                }
            };
        }
    }
}
