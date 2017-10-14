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
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return buffer.Properties.GetOrCreateSingletonProperty(() => new CommentTagger(buffer)) as ITagger<T>;
        }
    }
}
