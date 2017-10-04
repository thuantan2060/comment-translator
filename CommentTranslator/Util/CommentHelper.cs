namespace CommentTranslator.Util
{
    public class CommentHelper
    {
        public static int LineCount(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return text.Split('\n').Length;
        }
    }
}
