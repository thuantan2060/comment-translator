using CommentTranslator.Support;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornmentTagger : IntraTextAdornmentTagger<CommentTag, CommentAdornment>
    {
        #region Fields

        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, IEditorFormatMap format, Lazy<ITagAggregator<CommentTag>> commentTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new CommentAdornmentTagger(view, format, commentTagger.Value));
        }

        private ITagAggregator<CommentTag> _commentTagger;
        private IEditorFormatMap _format;

        #endregion

        #region Contructors

        public CommentAdornmentTagger(IWpfTextView view, IEditorFormatMap format, ITagAggregator<CommentTag> commentTagger) : base(view)
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

        protected override CommentAdornment CreateAdornment(CommentTag data, SnapshotSpan span, SnapshotSpan containSpan)
        {
            return new CommentAdornment(data, span, _view, _format, containSpan);
        }

        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, CommentTag, SnapshotSpan>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var snapshot = spans[0].Snapshot;
            var mappingCommentTagSpans = _commentTagger.GetTags(spans);
            var commentTagSpans = ConvertToTagSpan(snapshot, mappingCommentTagSpans);

            foreach (ITagSpan<CommentTag> dataTagSpan in commentTagSpans)
            {
                SnapshotSpan adornmentSpan = new SnapshotSpan(dataTagSpan.Span.Start, 0);
                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag, dataTagSpan.Span);
            }
        }

        protected override bool UpdateAdornment(CommentAdornment adornment, CommentTag data, SnapshotSpan span, SnapshotSpan containSpan)
        {
            adornment.Update(data, span, containSpan);
            return true;
        }

        private IEnumerable<ITagSpan<T>> ConvertToTagSpan<T>(ITextSnapshot snapshot, IEnumerable<IMappingTagSpan<T>> mappingTagSpans) where T : ITag
        {
            if (mappingTagSpans.Count() == 0) yield break;

            foreach (var mapTagSpan in mappingTagSpans)
            {
                var nssc = mapTagSpan.Span.GetSpans(snapshot);
                if (nssc.Count > 0)
                {
                    var snapshotSpan = nssc[0];

                    //string text = snapshotSpan.is.GetText();
                    if (snapshotSpan.IsEmpty)
                        continue;

                    yield return new TagSpan<T>(snapshotSpan, mapTagSpan.Tag);
                }
            }
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
