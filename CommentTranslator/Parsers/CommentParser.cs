using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommentTranslator.Parsers
{
    public abstract class CommentParser : ICommentParser
    {
        protected IEnumerable<CommentTag> Tags { get; set; }

        public IEnumerable<Comment> GetComment(SnapshotSpan span)
        {
            var watch = new Stopwatch();
            watch.Start();
            try
            {

                return Parse(span.GetText(), Tags);
            }
            finally
            {
                watch.Stop();
                Debug.WriteLine("Time: " + watch.ElapsedMilliseconds);
            }
        }

        //protected virtual Regex CreateRegex(IEnumerable<CommentTag> tags)
        //{
        //    if (tags.Count() == 0) return null;
        //    var patternBuilder = new StringBuilder();

        //    //Append tag to pattern
        //    foreach (var tag in tags)
        //    {
        //        patternBuilder.Append(string.Format("|(?<{0}>{1}((?!{2}).)*{2})", tag.Name, tag.Start, tag.End));
        //    }

        //    //Remove first '|'
        //    if (patternBuilder.Length > 0)
        //    {
        //        patternBuilder.Remove(0, 1);
        //    }

        //    return new Regex(patternBuilder.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        //}

        protected virtual IEnumerable<Comment> Parse(string text, IEnumerable<CommentTag> tags)
        {
            var comments = new List<Comment>();

            while (text.Length > 0)
            {
                //Find first start tag
                CommentTag currentTag = null;
                int startIndex = int.MaxValue;
                foreach (var tag in tags)
                {
                    var index = text.IndexOf(tag.Start);
                    if (index >= 0 && index < startIndex)
                    {
                        currentTag = tag;
                        startIndex = index;
                    }
                }

                //Check if found start tag
                if (currentTag != null)
                {
                    //Find first end tag
                    var endIndex = 0;
                    if (currentTag.Start != currentTag.End)
                    {
                        endIndex = text.IndexOf(currentTag.End);
                    }
                    else
                    {
                        endIndex = text.Substring(startIndex + currentTag.Start.Length).IndexOf(currentTag.End);
                    }

                    //Check if found end tag
                    if (endIndex >= 0)
                    {
                        //Add comment
                        comments.Add(new Comment()
                        {
                            Tag = currentTag,
                            Text = text.Substring(startIndex + currentTag.Start.Length, endIndex - startIndex - currentTag.Start.Length)
                        });

                        text = text.Substring(endIndex + currentTag.End.Length);
                    }
                    else
                    {
                        text = "";
                    }
                }
                else
                {
                    text = "";
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
