using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CommentTranslator.Ardonment
{
    public class CommentTranslateTag : ITag
    {
        public string Text { get; set; }
        public int TimeWaitAfterChange { get; set; }
        public IContentType ContentType { get; set; }

        public IClassificationType ClassificationType { get; set; }

        public CommentTranslateTag(string text, IContentType contentType, IClassificationType classificationType, int timeWaitAfterChange = 0)
        {
            Text = text;
            ContentType = contentType;
            ClassificationType = classificationType;
            TimeWaitAfterChange = timeWaitAfterChange;
        }
    }
}
