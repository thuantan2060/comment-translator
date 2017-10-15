using System;
using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class FSharpCommentParser : CommentParser
    {
        public FSharpCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Singleline comment
                new ParseTag()
                {
                    Start = "//",
                    End = Environment.NewLine,
                    Name = "singleline"
                },

                //Multi line comment
                new ParseTag(){
                    Start = "(*",
                    End = "*)",
                    Name = "multiline"
                }
            };
        }
    }
}
