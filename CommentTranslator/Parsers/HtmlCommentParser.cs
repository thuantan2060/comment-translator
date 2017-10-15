using System.Collections.Generic;

namespace CommentTranslator.Parsers
{
    public class HtmlCommentParser : CommentParser
    {
        public HtmlCommentParser()
        {
            Tags = new List<ParseTag>
            {
                //Multi line comment
                new ParseTag(){
                    Start = "<!--",
                    End = "-->",
                    Name = "multiline"
                }
            };
        }

        //public override Comment GetComment(Ardonment.CommentTag comment)
        //{
        //    //if (comment.Text.StartsWith("<!--") && comment.Text.EndsWith("-->"))
        //    //{
        //    //    return new Comment()
        //    //    {
        //    //        Origin = comment.Text,
        //    //        Line = 1,
        //    //        MarginTop = 0,
        //    //        Trimmed = "",
        //    //        Position = GetPositions(comment)
        //    //    };
        //    //}

        //    if (!comment.Text.StartsWith("<!--") && comment.Text.EndsWith("-->")) comment.Text = "<!--" + comment.Text;

        //    return base.GetComment(comment);
        //}
    }
}
