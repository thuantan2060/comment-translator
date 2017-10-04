using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
                        if (currentTag.End.Length == 0)
                        {
                            endIndex = text.IndexOf('\n');
                        }
                        else
                        {
                            endIndex = text.IndexOf(currentTag.End);
                        }
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

                        text = text.Substring(endIndex + (currentTag.End.Length > 0 ? currentTag.End.Length : 1));
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

        public virtual TrimComment TrimComment(string comment)
        {
            foreach (var tag in Tags)
            {
                var startIndex = comment.IndexOf(tag.Start);
                if (startIndex >= 0)
                {
                    //Shif start index
                    startIndex += tag.Start.Length;

                    //Calculate end index
                    var endIndex = tag.End.Length == 0 ? comment.Length - 1 : comment.IndexOf(tag.End);
                    if (startIndex >= endIndex)
                    {
                        return new TrimComment()
                        {
                            OriginText = comment,
                            LineCount = 0,
                            TrimedText = ""
                        };
                    }

                    //Break into lines
                    var text = comment.Substring(startIndex, endIndex - startIndex);
                    var lines = text.Split('\n');

                    //Check if single line
                    if (lines.Length <= 1)
                    {
                        return new TrimComment()
                        {
                            OriginText = comment,
                            LineCount = 1,
                            TrimedText = text.Trim()
                        };
                    }

                    //Trim multi line comment
                    var builder = new StringBuilder();

                    //Add first line
                    builder.AppendLine(lines[0].Trim());

                    //Add next lines
                    for (int i = 1; i < lines.Length; i++)
                    {
                        builder.AppendLine(lines[i].Trim());
                    }

                    var trimedText = builder.ToString().TrimEnd();
                    return new TrimComment()
                    {
                        OriginText = comment,
                        LineCount = CommentHelper.LineCount(trimedText),
                        TrimedText = trimedText
                    };
                }
            }

            return new TrimComment()
            {
                OriginText = comment,
                LineCount = CommentHelper.LineCount(comment),
                TrimedText = comment
            };
        }

        public string SimpleTrimComment(string comment)
        {
            var lines = comment.Split('\n');
            var builder = new StringBuilder();

            //Add first line
            builder.AppendLine(lines[0].Trim());

            //Add next lines
            for (int i = 1; i < lines.Length; i++)
            {
                builder.AppendLine(lines[i].Trim());
            }
            return builder.ToString().TrimEnd();
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
