using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CommentTranslator.Ardonment
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(CommentTag))]
    internal sealed class CommentTranslateTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return buffer.Properties.GetOrCreateSingletonProperty(() => new CommentTranslateTagger(buffer)) as ITagger<T>;
        }
    }
}
