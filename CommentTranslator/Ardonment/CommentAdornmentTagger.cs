using CommentTranslator.Support;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornmentTagger : IntraTextAdornmentTagger<CommentTranslateTag, CommentAdornment>
    {
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<CommentTranslateTag>> commentTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new CommentAdornmentTagger(view, commentTagger.Value));
        }

        private ITagAggregator<CommentTranslateTag> _commentTagger;


        public CommentAdornmentTagger(IWpfTextView view, ITagAggregator<CommentTranslateTag> commentTagger) : base(view)
        {
            _commentTagger = commentTagger;
        }

        public void Dispose()
        {
            _commentTagger.Dispose();

            //view.Properties.RemoveProperty(typeof(CommentTranslateTagger));
        }

        protected override CommentAdornment CreateAdornment(CommentTranslateTag data, SnapshotSpan span)
        {
            return new CommentAdornment(data, span, 0, 20);
        }

        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, CommentTranslateTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            ITextSnapshot snapshot = spans[0].Snapshot;

            var commentTags = _commentTagger.GetTags(spans);

            foreach (IMappingTagSpan<CommentTranslateTag> dataTagSpan in commentTags)
            {
                NormalizedSnapshotSpanCollection commentTagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if (commentTagSpans.Count != 1)
                    continue;

                SnapshotSpan adornmentSpan = new SnapshotSpan(commentTagSpans[0].Start, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override bool UpdateAdornment(CommentAdornment adornment, CommentTranslateTag data)
        {
            adornment.Update(data);
            return true;
        }
    }
}
