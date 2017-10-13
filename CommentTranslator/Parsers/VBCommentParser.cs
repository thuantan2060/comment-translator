using System;
using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class VBCommentParser : CommentParser
    {
        public VBCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Singleline comment
                new ParseTag()
                {
                    Start = "'",
                    End = Environment.NewLine,
                    Name = "singleline"
                },

                new ParseTag()
                {
                    Start = "'",
                    End = "",
                    Name = "singlelineend"
                },
            };
        }
    }
}
