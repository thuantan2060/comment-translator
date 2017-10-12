using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Text;

namespace CommentTranslator.Parsers
{
    public abstract class CommentParser : ICommentParser
    {
        protected IEnumerable<CommentTag> Tags { get; set; }


        //protected virtual IEnumerable<CommentRegion> Parse(string text, IEnumerable<CommentTag> tags)
        //{
        //    var comments = new List<Comment>();

        //    while (text.Length > 0)
        //    {
        //        //Find first start tag
        //        CommentTag currentTag = null;
        //        int startIndex = int.MaxValue;
        //        foreach (var tag in tags)
        //        {
        //            var index = text.IndexOf(tag.Start);
        //            if (index >= 0 && index < startIndex)
        //            {
        //                currentTag = tag;
        //                startIndex = index;
        //            }
        //        }

        //        //Check if found start tag
        //        if (currentTag != null)
        //        {
        //            //Cut text
        //            text = text.Substring(startIndex + currentTag.Start.Length);

        //            //Find first end tag
        //            var endIndex = 0;
        //            if (currentTag.Start != currentTag.End)
        //            {
        //                if (currentTag.End.Length == 0)
        //                {
        //                    endIndex = text.IndexOf('\n');
        //                }
        //                else
        //                {
        //                    endIndex = text.IndexOf(currentTag.End);
        //                }
        //            }
        //            else
        //            {
        //                endIndex = text.IndexOf(currentTag.End);
        //            }

        //            //Check if found end tag
        //            if (endIndex >= 0)
        //            {
        //                var originText = text.Substring(0, endIndex);
        //                var trimmed = TrimComment(originText);

        //                //Add comment
        //                comments.Add(new Comment()
        //                {
        //                    Tag = currentTag,
        //                    Origin = originText,
        //                    Trimmed = trimmed.Text,
        //                    Line = trimmed.Line,
        //                    MarginTop = trimmed.MarginTop
        //                });

        //                text = text.Substring(endIndex + (currentTag.End.Length > 0 ? currentTag.End.Length : 1));
        //            }
        //            else
        //            {
        //                text = "";
        //            }
        //        }
        //        else
        //        {
        //            text = "";
        //        }
        //    }

        //    return comments;
        //}

        //public virtual Comment GetComment(Ardonment.CommentTag comment)
        //{
        //    if (IsValidComment(comment.Text))
        //    {
        //        var trimmed = TrimComment(comment.Text);
        //        return new Comment()
        //        {
        //            Origin = comment.Text,
        //            Trimmed = trimmed.Text,
        //            Line = trimmed.Line,
        //            MarginTop = trimmed.MarginTop,
        //            Position = GetPositions(comment)
        //        };
        //    }

        //    return new Comment()
        //    {
        //        Origin = comment.Text,
        //        Trimmed = "",
        //        Line = 1,
        //        MarginTop = 0,
        //        Position = GetPositions(comment)
        //    };
        //}

        public virtual string TrimCommentLines(string comment)
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

        public virtual TextPositions GetPositions(Ardonment.CommentTag comment)
        {
            if (CommentHelper.LineCount(comment.Text) > 1)
            {
                return TextPositions.Right;
            }

            return TextPositions.Bottom;
        }

        public virtual TrimmedText TrimComment(string comment)
        {
            //var lines = comment.Split('\n');

            ////Check if single line
            //if (lines.Length <= 1)
            //{
            //    return new TrimmedText(comment.Trim(), 1, 0);
            //}

            ////Trim multi line comment
            //var builder = new StringBuilder();

            ////Add first line
            //builder.AppendLine(lines[0].Trim());

            ////Add next lines
            //for (int i = 1; i < lines.Length; i++)
            //{
            //    builder.AppendLine(lines[i].Trim());
            //}

            //var trimmedComment = builder.ToString().TrimEnd();
            //return new TrimmedText(trimmedComment, comment.EndsWith("\n") ? lines.Length - 1 : lines.Length, MarginTop(trimmedComment));
            foreach (var tag in Tags)
            {
                var startIndex = comment.IndexOf(tag.Start);
                if (startIndex >= 0)
                {
                    //Shiff start index
                    startIndex += tag.Start.Length;

                    //Calculate end index
                    var endIndex = 0;
                    if (tag.End.Length == 0)
                        endIndex = comment.EndsWith("\n") ? comment.Length - 1 : comment.Length;
                    else
                        endIndex = comment.IndexOf(tag.End);

                    if (startIndex >= endIndex)
                    {
                        return new TrimmedText("");
                    }

                    //Break into lines
                    var text = comment.Substring(startIndex, endIndex - startIndex);
                    var lines = text.Split('\n');

                    //Check if single line
                    if (lines.Length <= 1)
                    {
                        return new TrimmedText(text.Trim(), 1, 0);
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

                    var trimmedComment = builder.ToString().TrimEnd();
                    return new TrimmedText(trimmedComment, comment.EndsWith("\n") ? lines.Length - 1 : lines.Length, MarginTop(trimmedComment));
                }
            }

            return new TrimmedText(comment);
        }

        protected virtual int MarginTop(string text)
        {
            var lines = text.Split('\n');
            var index = 0;
            while (index < lines.Length && string.IsNullOrEmpty(lines[index].Trim()))
            {
                index++;
            }

            return index;
        }

        public IEnumerable<CommentRegion> GetCommentRegions(ITextSnapshot snapshot)
        {
            return GetCommentRegions(snapshot, Tags);
        }

        protected virtual IEnumerable<CommentRegion> GetCommentRegions(ITextSnapshot snapshot, IEnumerable<CommentTag> tags)
        {
            var comments = new List<CommentRegion>();
            var text = snapshot.GetText();
            var offset = 0;

            while (text.Length > 0)
            {
                //Find first start tag
                var indexTags = GetIndexTags(text, tags);

                //Stop if not found tag
                if (indexTags == null) break;

                //Try for each tag
                foreach (var tag in indexTags.Tags)
                {
                    var trimStart = text.Substring(indexTags.Index + tag.Start.Length);

                    //Find end index
                    var endIndex = 0;
                    if (tag.Start != tag.End)
                    {
                        endIndex = trimStart.IndexOf(tag.End);
                    }
                    else
                    {
                        endIndex = trimStart.IndexOf(tag.End);
                    }

                    //Found end index
                    if (endIndex >= 0)
                    {
                        var commentRegion = new CommentRegion()
                        {
                            Start = offset + indexTags.Index,
                            Length = tag.Start.Length + endIndex + tag.End.Length
                        };

                        offset = commentRegion.Start + commentRegion.Length;
                        text = trimStart.Substring(endIndex + tag.End.Length);
                        comments.Add(commentRegion);

                        break;
                    }
                }
            }

            return comments;
        }

        //public bool IsValidComment(string comment)
        //{
        //    foreach (var tag in Tags)
        //    {
        //        if (comment.StartsWith(tag.Start) && comment.EndsWith(tag.End))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        private IndexTags GetIndexTags(string text, IEnumerable<CommentTag> tags)
        {
            var indexTagsDic = new Dictionary<int, IndexTags>();
            int minIndex = int.MaxValue;

            foreach (var tag in tags)
            {
                var index = text.IndexOf(tag.Start);
                if (index >= 0 && index <= minIndex)
                {
                    minIndex = index;

                    if (indexTagsDic.ContainsKey(index))
                    {
                        indexTagsDic[index].Tags.Add(tag);
                    }
                    else
                    {
                        indexTagsDic.Add(index, new IndexTags()
                        {
                            Index = index,
                            Tags = new List<CommentTag>()
                            {
                                tag
                            }
                        });
                    }
                }
            }

            return indexTagsDic.ContainsKey(minIndex) ? indexTagsDic[minIndex] : null;
        }

        private class IndexTags
        {
            public int Index { get; set; }
            public List<CommentTag> Tags { get; set; }
        }
    }

    public class Comment
    {
        public CommentTag Tag { get; set; }
        public string Origin { get; set; }
        public string Trimmed { get; set; }
        public TextPositions Position { get; set; }
        public int Line { get; set; }
        public int MarginTop { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class CommentTag
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Name { get; set; }
    }

    public class TrimmedText
    {
        public string Text { get; set; }
        public int Line { get; set; }
        public int MarginTop { get; set; }

        public TrimmedText(string text, int line, int marginTop)
        {
            this.Text = text;
            this.Line = line;
            this.MarginTop = marginTop;
        }

        public TrimmedText(string text)
        {
            this.Text = text;
            this.Line = CommentHelper.LineCount(text);
        }
    }
}
