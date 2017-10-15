using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommentTranslator.Parsers
{
    public abstract class CommentParser : ICommentParser
    {
        #region Fields

        protected IEnumerable<ParseTag> Tags { get; set; }

        #endregion

        #region Contructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public virtual Comment GetComment(string commentText)
        {
            var text = commentText;

            //Remove tags
            foreach (var tag in Tags)
            {
                if (commentText.StartsWith(tag.Start) && commentText.EndsWith(tag.End))
                {
                    text = text.Substring(tag.Start.Length, commentText.Length - tag.Start.Length - tag.End.Length);
                    break;
                }
            }

            //Split text to lines
            var lines = text.Split('\n');
            var builder = new StringBuilder();

            //Trim each line
            for (int i = 0; i < lines.Length; i++)
            {
                builder.AppendLine(lines[i].Trim());
            }

            //Calculate margin top
            var marginTop = GetMarginTop(lines);

            //Get content
            text = builder.ToString().Trim();

            //Create comment
            var comment = new Comment()
            {
                Origin = commentText,
                Content = text,
                Line = lines.Length - (commentText.EndsWith(Environment.NewLine) ? 1 : 0),
                MarginTop = marginTop
            };

            //Get position
            comment.Position = GetPositions(comment);

            return comment;
        }

        public virtual TextPositions GetPositions(Comment comment)
        {
            if (comment.Line > 1)
            {
                return TextPositions.Right;
            }

            return TextPositions.Bottom;
        }

        public IEnumerable<CommentRegion> GetCommentRegions(ITextSnapshot snapshot, int startFrom = 0)
        {
            return GetCommentRegions(snapshot, Tags, startFrom);
        }

        public IEnumerable<CommentRegion> GetCommentRegions(string text, int startFrom = 0)
        {
            return GetCommentRegions(text, Tags, startFrom);
        }

        #endregion

        #region Functions

        protected virtual int GetMarginTop(string[] lines)
        {
            var index = 0;
            while (index < lines.Length && string.IsNullOrEmpty(lines[index].Trim()))
            {
                index++;
            }

            return index;
        }

        protected virtual IEnumerable<CommentRegion> GetCommentRegions(ITextSnapshot snapshot, IEnumerable<ParseTag> tags, int startFrom = 0)
        {
            var text = snapshot.GetText().Substring(startFrom);
            return GetCommentRegions(text, tags, startFrom);
        }

        protected virtual IEnumerable<CommentRegion> GetCommentRegions(string text, IEnumerable<ParseTag> tags, int startFrom = 0)
        {
            var comments = new List<CommentRegion>();
            if (!string.IsNullOrEmpty(text))
            {
                var offset = startFrom;
                while (text.Length > 0)
                {
                    //Find first start tag
                    var indexTags = GetIndexTags(text, tags);

                    //Stop if not found tag
                    if (indexTags == null) break;

                    //Try for each tag
                    var foundTag = false;
                    foreach (var tag in indexTags.Tags)
                    {
                        var trimStart = text.Substring(indexTags.Index + tag.Start.Length);

                        //Find end index
                        var endIndex = 0;
                        if (tag.Start != tag.End)
                        {
                            endIndex = string.IsNullOrEmpty(tag.End) ? trimStart.Length : trimStart.IndexOf(tag.End);
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
                            text = endIndex < trimStart.Length ? trimStart.Substring(endIndex + tag.End.Length) : "";
                            comments.Add(commentRegion);
                            foundTag = true;

                            break;
                        }
                    }

                    if (!foundTag) break;
                }
            }

            return comments;
        }

        private IndexTags GetIndexTags(string text, IEnumerable<ParseTag> tags)
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
                            Tags = new List<ParseTag>()
                            {
                                tag
                            }
                        });
                    }
                }
            }

            return indexTagsDic.ContainsKey(minIndex) ? indexTagsDic[minIndex] : null;
        }

        #endregion

        #region InnerMembers

        private class IndexTags
        {
            public int Index { get; set; }
            public List<ParseTag> Tags { get; set; }
        }

        #endregion
    }


    public class ParseTag
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Name { get; set; }
    }
}
