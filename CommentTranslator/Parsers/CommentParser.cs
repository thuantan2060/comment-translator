using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommentTranslator.Parsers
{
    public  abstract class CommentParser : ICommentParser
    {
        protected IEnumerable<CommentTag> Tags { get; set; }
        protected Regex Regex { get; set; }

        public IEnumerable<Comment> GetComment(SnapshotSpan span)
        {
            if (Regex == null)
            {
                Regex = CreateRegex(Tags);
            }

            return Parse(span.GetText(), Tags, Regex);
        }

        protected virtual Regex CreateRegex(IEnumerable<CommentTag> tags)
        {
            if (tags.Count() == 0) return null;
            var patternBuilder = new StringBuilder();

            //Append tag to pattern
            foreach (var tag in tags)
            {
                patternBuilder.Append(string.Format("|(?<{0}>{1}.*{2})", tag.Name, tag.Start, tag.End));
            }

            //Remove first '|'
            if (patternBuilder.Length > 0)
            {
                patternBuilder.Remove(0, 1);
            }

            return new Regex(patternBuilder.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        protected virtual IEnumerable<Comment> Parse(string text, IEnumerable<CommentTag> tags, Regex regex)
        {
            var comments = new List<Comment>();
            var matches = regex.Matches(text);
            
            foreach(Match match in matches)
            {
                foreach(var tag in tags)
                {
                    var group = match.Groups[tag.Name];
                    if (group != null) {
                        comments.Add(new Comment() {
                            Text = group.Value,
                            Tag = tag
                        });
                    }
                }
            }

            return comments;
        }
    }

    public class Comment
    {
        public string Text { get; set; }
        public CommentTag Tag { get; set; }
    }

    public class CommentTag
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Name { get; set; }
    }
}
