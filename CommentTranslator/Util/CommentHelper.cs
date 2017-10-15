namespace CommentTranslator.Util
{
    public class CommentHelper
    {
        public static int LineCount(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            var lines = text.Split('\n');
            return text.EndsWith("\n") ? lines.Length - 1 : lines.Length;
        }
    }
}
