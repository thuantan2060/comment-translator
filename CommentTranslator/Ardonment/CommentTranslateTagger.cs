using CommentTranslator.Support;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentTranslateTagger : RegexTagger<CommentTag>
    {
        internal CommentTranslateTagger(ITextBuffer buffer) : base(buffer, new[] { new Regex(@"(?<comment>//.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        //: base(buffer, new[] { new Regex(@"\b[\dA-F]{6}\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override CommentTag TryCreateTagForMatch(Match match)
        {
            if (match.Groups.Count > 0)
            {
                var text = match.Groups["comment"].Value;

                if (!string.IsNullOrEmpty(text))
                {
                    return new CommentTag(text, null, null, 500);
                }
            }

            return null;
        }
    }
}
