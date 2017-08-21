using System.Text;
using System.Text.RegularExpressions;

namespace CommentTranslator.Util
{
    public class CommentHelper
    {
        public static string TrimCommnentSingleline(string text)
        {
            //Trim text
            text = text.Trim();

            //Trim sumary
            if (text.StartsWith("///"))
            {
                text = text.Substring(3, text.Length - 3);

            }

            //Trim single line comment
            if (text.StartsWith("//"))
            {
                text = text.Substring(2, text.Length - 2);
            }

            return text.Trim();
        }

        public static string TrimComment(string text)
        {
            //Trim text
            text = text.Trim();

            //Trim Csharp multiline comment
            if (text.StartsWith("/*") && text.EndsWith("*/"))
            {
                text = text.Substring(2, text.Length - 4);
            }

            //Trim html multiline comment
            if (text.StartsWith("<!--") && text.EndsWith("-->"))
            {
                text = text.Substring(4, text.Length - 5);
            }

            //Check multiline
            string[] lines = Regex.Split(text, @"\r\n");
            if (lines.Length > 1)
            {
                var builder = new StringBuilder();

                foreach(var line in lines)
                {
                    builder.AppendLine(TrimCommnentSingleline(line));
                }

                return builder.ToString();
            }
            else
            {
                return TrimCommnentSingleline(text);
            }
        }
    }
}
