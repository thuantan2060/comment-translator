using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CommentTranslator.Presentation
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class TranslatePopupConnector : IWpfTextViewCreationListener
    {
        // Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("TransplatePopupAdornment")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        [Order(After = PredefinedAdornmentLayers.Outlining)]
        [Order(After = PredefinedAdornmentLayers.Selection)]
        [Order(After = PredefinedAdornmentLayers.Squiggle)]
        [Order(After = PredefinedAdornmentLayers.Text)]
        [Order(After = PredefinedAdornmentLayers.TextMarker)]
        private AdornmentLayerDefinition editorAdornmentLayer;

#pragma warning restore 649, 169

        /// <summary>
        /// Instantiates a TransplatePopupAdornment manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            TranslatePopupAdornment.Create(textView);
        }

        /// <summary>
        /// Translates the specified text.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="text">The text.</param>
        public static void Translate(IWpfTextView view, string text)
        {
            TranslatePopupAdornment adornment = null;

            try
            {
                adornment = view.Properties.GetProperty<TranslatePopupAdornment>(typeof(TranslatePopupAdornment));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (adornment != null)
            {
                adornment.Translate(view.Selection.SelectedSpans[0], text);
            }
        }
    }
}
