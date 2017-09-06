using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace CommentTranslator.Support
{
    public abstract class RegexMultiLineTagger<T> : ITagger<T> where T : ITag
    {
        private IEnumerable<RegexInfo> _regexInfos;
        private ITextBuffer _textBuffer;
        private IEnumerable<Regex> _matchExpressions;

        public RegexMultiLineTagger(ITextBuffer buffer, IEnumerable<RegexInfo> regexInfos)
        {
            _regexInfos = regexInfos;
            _textBuffer = buffer;

            _matchExpressions = regexInfos.Select(r => new Regex(r.GetPattern(), RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline)).ToList();
            _textBuffer.Changed += (sender, args) => HandleBufferChanged(args);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            throw new NotImplementedException();
        }

        protected abstract T TryCreateTagForMatch(Match match);

        private void HandleBufferChanged(TextContentChangedEventArgs args)
        {
            if (args.Changes.Count == 0)
                return;

            var temp = TagsChanged;
            if (temp == null)
                return;

            // Combine all changes into a single span so that
            // the ITagger<>.TagsChanged event can be raised just once for a compound edit
            // with many parts.

            ITextSnapshot snapshot = args.After;

            int start = args.Changes[0].NewPosition;
            int end = args.Changes[args.Changes.Count - 1].NewEnd;

            SnapshotSpan totalAffectedSpan = new SnapshotSpan(
                snapshot.GetLineFromPosition(start).Start,
                snapshot.GetLineFromPosition(end).End);

            temp(this, new SnapshotSpanEventArgs(totalAffectedSpan));
        }
    }

    public abstract class RegexInfo
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string GetPattern()
        {
            return string.Format("{0}(.*){1}", Start, End);
        }
    }
}
