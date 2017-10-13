using CommentTranslator.Ardonment;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace CommentTranslator.Util
{
    internal static class CommentTagHelper
    {
        public static IEnumerable<ITagSpan<CommentTag>> GetCommentTag(this ITagAggregator<IClassificationTag> aggregator, NormalizedSnapshotSpanCollection spans)
        {
            //Get snapshot
            var snapshot = spans[0].Snapshot;
            var contentType = snapshot.TextBuffer.ContentType;
            if (!(contentType.IsOfType("code") || contentType.IsOfType("projection")))
                yield break;

            if (CommentParserHelper.GetCommentParser(contentType.TypeName) == null)
                yield break;

            foreach (var tagSpan in aggregator.GetTags(spans))
            {
                // find spans that the language service has already classified as comments ...
                string classificationName = tagSpan.Tag.ClassificationType.Classification;
                if (classificationName.IndexOf("comment", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                var nssc = tagSpan.Span.GetSpans(snapshot);
                if (nssc.Count > 0)
                {
                    var snapshotSpan = nssc[0];

                    string text = snapshotSpan.GetText();
                    if (String.IsNullOrWhiteSpace(text))
                        continue;

                    yield return new TagSpan<CommentTag>(snapshotSpan, new CommentTag(text, CommentParserHelper.GetCommentParser(contentType.TypeName), 200));
                }
            }
        }
    }
}
