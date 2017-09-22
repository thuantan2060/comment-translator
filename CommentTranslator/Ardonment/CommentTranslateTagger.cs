using CommentTranslator.Support;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentTranslateTagger : RegexTagger<CommentTranslateTag>
    {
        internal CommentTranslateTagger(ITextBuffer buffer) : base(buffer, new[] { new Regex(@"(?<comment>//.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
            //: base(buffer, new[] { new Regex(@"\b[\dA-F]{6}\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override CommentTranslateTag TryCreateTagForMatch(Match match)
        {
            if (match.Groups.Count > 0)
            {
                var text = match.Groups["comment"].Value;

                if (!string.IsNullOrEmpty(text))
                {
                    return new CommentTranslateTag(text, 500);
                }
            }

            return null;
        }
    }
}
