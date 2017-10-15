using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CommentTranslator.Ardonment
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [ContentType("projection")]
    [Order(Before = PredefinedAdornmentLayers.Caret)]
    [TagType(typeof(CommentTag))]
    public sealed class CommentTaggerProvider : ITaggerProvider
    {

#pragma warning disable 649 // "field never assigned to" -- field is set by MEF.

        [Import]
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;

#pragma warning restore 649

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return buffer.Properties.GetOrCreateSingletonProperty(() => new CommentTagger(buffer, BufferTagAggregatorFactoryService.CreateTagAggregator<IClassificationTag>(buffer))) as ITagger<T>;
        }
    }
}
