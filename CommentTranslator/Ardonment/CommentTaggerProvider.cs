using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentTranslator.Ardonment
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(CommentTranslateTag))]
    public sealed class CommentTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService Aggregator = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return buffer.Properties.GetOrCreateSingletonProperty(() => new CommentTagger(ClassificationRegistry, Aggregator.CreateTagAggregator<IClassificationTag>(buffer))) as ITagger<T>;
        }
    }
}
