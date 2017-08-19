using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CommentTranslator.Presentation
{
    [Export(typeof(IMouseProcessorProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Name("TranslateMouseProcessorProvider")]
    internal sealed class TranslateMouseProcessorProvider : IMouseProcessorProvider
    {
        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new TranslateMouseProcessor(wpfTextView);
        }
    }
}
