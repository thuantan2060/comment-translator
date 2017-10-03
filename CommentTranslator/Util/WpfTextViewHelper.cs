using Microsoft.VisualStudio.Text.Editor;

namespace CommentTranslator.Util
{
    public static class WpfTextViewHelper
    {
        public static double GetLineHeight(this IWpfTextView view)
        {
            var height = 20d;
            try
            {
                height = view.LineHeight;
            }
            catch { }

            return height;
        }
    }
}
