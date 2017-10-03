using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class VBCommentParser : CommentParser
    {
        public VBCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //Singleline comment
                new CommentTag()
                {
                    Start = "'",
                    End = "\n",
                    Name = "singleline"
                },
            };
        }
    }
}
