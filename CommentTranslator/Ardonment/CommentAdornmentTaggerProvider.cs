using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CommentTranslator.Ardonment
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [ContentType("projection")]
    [Order(After = PredefinedAdornmentLayers.Caret)]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class CommentAdornmentTaggerProvider : IViewTaggerProvider
    {
#pragma warning disable 649 // "field never assigned to" -- field is set by MEF.
        [Import]
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;

        [Import]
        internal IEditorFormatMapService FormatMapService; // MEF
#pragma warning restore 649

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                throw new ArgumentNullException("textView");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (buffer != textView.TextBuffer)
                return null;

            return CommentAdornmentTagger.GetTagger(
                (IWpfTextView)textView,
                FormatMapService.GetEditorFormatMap(textView),
                new Lazy<ITagAggregator<CommentTag>>(() => BufferTagAggregatorFactoryService.CreateTagAggregator<CommentTag>(textView.TextBuffer))
                ) as ITagger<T>;
        }
    }
}
