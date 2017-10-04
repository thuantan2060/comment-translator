using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CommentTranslator.Ardonment
{
    internal class CommentTranslateTag : ITag
    {
        public string Text { get; set; }
        public int TimeWaitAfterChange { get; set; }
        public IContentType ContentType { get; set; }

        public CommentTranslateTag(string text, IContentType contentType, int timeWaitAfterChange = 0)
        {
            Text = text;
            ContentType = contentType;
            TimeWaitAfterChange = timeWaitAfterChange;
        }
    }
}
