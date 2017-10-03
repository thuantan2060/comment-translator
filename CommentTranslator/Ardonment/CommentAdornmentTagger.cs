using CommentTranslator.Support;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornmentTagger : IntraTextAdornmentTagger<CommentTranslateTag, CommentAdornment>
    {
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<IClassificationTag>> commentTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new CommentAdornmentTagger(view, commentTagger.Value));
        }

        private ITagAggregator<IClassificationTag> _commentTagger;

        public CommentAdornmentTagger(IWpfTextView view, ITagAggregator<IClassificationTag> commentTagger) : base(view)
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
            var lineHeight = 20d;
            try
            {
                lineHeight = _view.LineHeight;
            }
            catch { }

            return new CommentAdornment(data, span, lineHeight);
        }

        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, CommentTranslateTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var commentTags = _commentTagger.GetCommentTag(spans);

            foreach (ITagSpan<CommentTranslateTag> dataTagSpan in commentTags)
            {
                SnapshotSpan adornmentSpan = new SnapshotSpan(dataTagSpan.Span.Start, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override bool UpdateAdornment(CommentAdornment adornment, CommentTranslateTag data, SnapshotSpan span)
        {
            var lineHeight = 20d;
            try
            {
                lineHeight = _view.LineHeight;
            }
            catch { }

            adornment.Update(data);
            return true;
        }
    }
}
