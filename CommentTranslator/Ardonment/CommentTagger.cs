using CommentTranslator.Parsers;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommentTranslator.Ardonment
{
    internal class CommentTagger : ITagger<CommentTag>
    {
        private ITextBuffer _buffer;
        private ITextSnapshot _snapshot;
        private IEnumerable<CommentRegion> _regions;
        private ICommentParser _parser;

        public CommentTagger(ITextBuffer buffer)
        {
            this._buffer = buffer;
            this._snapshot = buffer.CurrentSnapshot;
            this._regions = new List<CommentRegion>();
            this._parser = CommentParserHelper.GetCommentParser(buffer.ContentType.TypeName);

            if (_parser != null)
            {
                this.ReParse(null);
                this._buffer.Changed += BufferChanged;
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != _buffer.CurrentSnapshot)
                return;
            this.ReParse(e.Changes);
        }

        void ReParse(INormalizedTextChangeCollection changes)
        {
            //Check is enable auto translate
            if (!CommentTranslatorPackage.Settings.AutoTranslateComment) return;

            //Find last comment end
            var startFrom = 0;
            var newRegions = new List<CommentRegion>();
            var newSnapshot = _buffer.CurrentSnapshot;

            if (changes != null && changes.Count > 0)
            {
                //Find start of change line
                //var change = changes[0];
                //var changLine = newSnapshot.GetLineFromPosition(change.OldPosition);

                foreach (var region in this._regions.OrderBy(r => r.Start))
                {
                    if (region.Start + region.Length < changes[0].OldPosition)
                    {
                        newRegions.Add(region);
                    }
                    else
                    {
                        startFrom = newRegions.Count > 0 ? newRegions[newRegions.Count - 1].Start + newRegions[newRegions.Count - 1].Length : 0;
                        break;
                    }
                }
            }

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

        public IEnumerable<ITagSpan<CommentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!CommentTranslatorPackage.Settings.AutoTranslateComment || spans.Count == 0 || _parser == null)
                yield break;

            var currentRegions = this._regions;
            var currentSnapshot = this._snapshot;
            var entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);

            //Debug.WriteLine("GetTags: ({0}, {1}) -> {2}/{3}",
            //           entire.Start.Position,
            //           entire.End.Position,
            //           currentRegions.Where(x => x.Start <= entire.Start && x.Length + x.Start >= entire.End).Count(),
            //           currentRegions.Count());

            foreach (var region in currentRegions)
            {
                if (entire.OverlapsWith(new Span(region.Start, region.Length)))
                {
                    var span = new SnapshotSpan(currentSnapshot, region.Start, region.Length);
                    var tag = new CommentTag(span.GetText(), _parser, 200);

                    yield return new TagSpan<CommentTag>(span, tag);
                }
            }
        }

        private static SnapshotSpan AsSnapshotSpan(CommentRegion region, ITextSnapshot snapshot)
        {
            return new SnapshotSpan(snapshot, region.Start, region.Length);
        }
    }
}
