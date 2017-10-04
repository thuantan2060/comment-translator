using CommentTranslator.Support;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornmentTagger : IntraTextAdornmentTagger<CommentTranslateTag, CommentAdornment>
    {
        #region Fields

        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, IEditorFormatMap format, Lazy<ITagAggregator<IClassificationTag>> commentTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new CommentAdornmentTagger(view, format, commentTagger.Value));
        }

        private ITagAggregator<IClassificationTag> _commentTagger;
        private IEditorFormatMap _format;

        #endregion

        #region Contructors

        public CommentAdornmentTagger(IWpfTextView view, IEditorFormatMap format, ITagAggregator<IClassificationTag> commentTagger) : base(view)
        {
            _commentTagger = commentTagger;
            _format = format;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Dispose()
        {
            _commentTagger.Dispose();
        }

        #endregion

        #region Functions

        protected override CommentAdornment CreateAdornment(CommentTranslateTag data, SnapshotSpan span)
        {
            return new CommentAdornment(data, span, _view, _format);
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
            adornment.Update(data, span);
            return true;
        }

        #endregion

        #region Events

        #endregion

        #region EventHandlers

        #endregion

        #region InnerMembers

        #endregion
    }
}
