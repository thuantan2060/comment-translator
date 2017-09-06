using Microsoft.VisualStudio.Text.Tagging;

namespace CommentTranslator.Ardonment
{
    internal class CommentTranslateTag : ITag
    {
        public string Text { get; set; }
        public int TimeWaitAfterChange { get; set; }

        public CommentTranslateTag(string text, int timeWaitAfterChange = 0)
        {
            Text = text;
            TimeWaitAfterChange = timeWaitAfterChange;
        }
    }
}
