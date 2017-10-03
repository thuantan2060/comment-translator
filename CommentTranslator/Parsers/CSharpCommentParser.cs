using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class CSharpCommentParser : CommentParser
    {
        public CSharpCommentParser()
        {
            Tags = new List<CommentTag>
            {
                //XML tags comment
                new CommentTag()
                {
                    Start = "///",
                    End = "\n",
                    Name = "xmldoc"
                },

                //Singleline comment
                new CommentTag()
                {
                    Start = "//",
                    End = "\n",
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
