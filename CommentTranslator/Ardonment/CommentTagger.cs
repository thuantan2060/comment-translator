using CommentTranslator.Parsers;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using RangeTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommentTranslator.Ardonment
{
    internal class CommentTagger : ITagger<CommentTag>
    {
        #region Fields

        private ITextBuffer _buffer;
        private ITextSnapshot _snapshot;
        private IEnumerable<CommentRegion> _regions;
        private ICommentParser _parser;
        private ITagAggregator<IClassificationTag> _classificationTag;

        #endregion

        #region Contructors

        public CommentTagger(ITextBuffer buffer, ITagAggregator<IClassificationTag> classificationTag)
        {
            this._buffer = buffer;
            this._snapshot = buffer.CurrentSnapshot;
            this._regions = new List<CommentRegion>();
            this._parser = CommentParserHelper.GetCommentParser(buffer.ContentType.TypeName);
            this._classificationTag = classificationTag;

            if (_parser != null)
            {
                this.ReParse(null);
                this._buffer.Changed += BufferChanged;
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public IEnumerable<ITagSpan<CommentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!CommentTranslatorPackage.Settings.AutoTranslateComment || spans.Count == 0 || _parser == null)
                yield break;

            var currentRegions = this._regions;
            var currentSnapshot = this._snapshot;
            var entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);

            var commentTagSpans = _classificationTag.GetTagSpan(spans, "comment");
            var rangeItems = commentTagSpans.Select(tp => new RangeItem(tp.Span.Start.Position, tp.Span.End.Position));
            var ranges = new RangeTree<int, RangeItem>(new RangeItemComparer());
            if (rangeItems.Count() > 0)
            {
                ranges.Add(rangeItems);
            }

            //Debug.WriteLine("GetTags: ({0}, {1}) -> {2}/{3}",
            //           entire.Start.Position,
            //           entire.End.Position,
            //           currentRegions.Where(x => x.Start <= entire.Start && x.Length + x.Start >= entire.End).Count(),
            //           currentRegions.Count());

            foreach (var region in currentRegions)
            {
                if (entire.OverlapsWith(new Span(region.Start, region.Length)) && ranges.Query(new Range<int>(region.Start, region.End)).Count > 0)
                {
                    var span = new SnapshotSpan(currentSnapshot, region.Start, region.Length);
                    var tag = new CommentTag(span.GetText(), _parser, 200);

                    yield return new TagSpan<CommentTag>(span, tag);
                }
            }
        }


        #endregion

        #region Functions

        private void ReParse(INormalizedTextChangeCollection changes)
        {
            //Check is enable auto translate
            if (!CommentTranslatorPackage.Settings.AutoTranslateComment) return;

            //Find last comment end
            var newRegions = new List<CommentRegion>();
            var newSnapshot = _buffer.CurrentSnapshot;

            if (changes != null)
            {
                //Not run if no change
                if (changes.Count == 0) return;

                //Find start of change line
                foreach (var region in this._regions)
                {
                    if (region.Start + region.Length >= changes[0].OldPosition)
                    {
                        break;
                    }

                    newRegions.Add(region);
                }
            }

            //Find start position
            var startFrom = newRegions.Count > 0 ? newRegions[newRegions.Count - 1].Start + newRegions[newRegions.Count - 1].Length : 0;

            //Find new region
            var regions = _parser.GetCommentRegions(newSnapshot, startFrom);
            if (regions.Count() > 0) newRegions.AddRange(regions);

            //determine the changed span, and send a changed event with the new spans
            List<Span> oldSpans = new List<Span>(this._regions.Select(r => AsSnapshotSpan(r, this._snapshot)
                                                                            .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                                                                            .Span));
            List<Span> newSpans = new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);

            //the changed regions are regions that appear in one set or the other, but not both.
            NormalizedSpanCollection removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            int changeStart = int.MaxValue;
            int changeEnd = -1;

            if (removed.Count > 0)
            {
                changeStart = removed[0].Start;
                changeEnd = removed[removed.Count - 1].End;
            }

            if (newSpans.Count > 0)
            {
                changeStart = Math.Min(changeStart, newSpans[0].Start);
                changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
            }

            this._snapshot = newSnapshot;
            this._regions = newRegions;

            if (changeStart <= changeEnd)
            {
                //Debug.WriteLine("ReParse: ({0}, {1})", changeStart, changeEnd);

                ITextSnapshot snap = this._snapshot;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(this._snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        private static SnapshotSpan AsSnapshotSpan(CommentRegion region, ITextSnapshot snapshot)
        {
            return new SnapshotSpan(snapshot, region.Start, region.Length);
        }

        #endregion

        #region Events

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        #endregion

        #region EventHandlers

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != _buffer.CurrentSnapshot)
                return;
            this.ReParse(e.Changes);
        }

        #endregion

        #region InnerMembers

        private class RangeItemComparer : IComparer<RangeItem>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
            public int Compare(RangeItem x, RangeItem y)
            {
                return x.Range.CompareTo(y.Range);
            }
        }
        private class RangeItem : IRangeProvider<int>
        {
            public Range<int> Range { get; private set; }
            public int Start { get { return Range.From; } }
            public int End { get { return Range.To; } }

            public RangeItem(int start, int end)
            {
                Range = new Range<int>(start, end);
            }
        }

        #endregion
    }
}
