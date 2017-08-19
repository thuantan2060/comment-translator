using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Input;

namespace CommentTranslator.Presentation
{
    internal sealed class TranslateMouseProcessor : IMouseProcessor
    {
        private readonly TranslatePopupAdornment _translateAdornment;

        public TranslateMouseProcessor(IWpfTextView view)
        {
            try
            {
                _translateAdornment = view.Properties.GetProperty<TranslatePopupAdornment>(typeof(TranslatePopupAdornment));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_translateAdornment != null)
            {
                _translateAdornment.HandleMouseLeftButton(e);
            }
        }

        public void PostprocessMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (_translateAdornment != null)
            {
                _translateAdornment.HandleMouseRightButton(e);
            }
        }

        #region Unused events

        public void PostprocessMouseRightButtonUp(MouseButtonEventArgs e)
        {

        }

        public void PostprocessDragEnter(DragEventArgs e)
        {

        }

        public void PostprocessDragLeave(DragEventArgs e)
        {

        }

        public void PostprocessDragOver(DragEventArgs e)
        {

        }

        public void PostprocessDrop(DragEventArgs e)
        {

        }

        public void PostprocessGiveFeedback(GiveFeedbackEventArgs e)
        {

        }

        public void PostprocessMouseDown(MouseButtonEventArgs e)
        {

        }

        public void PostprocessMouseEnter(MouseEventArgs e)
        {

        }

        public void PostprocessMouseLeave(MouseEventArgs e)
        {

        }

        public void PostprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {

        }

        public void PostprocessMouseMove(MouseEventArgs e)
        {

        }

        public void PostprocessMouseUp(MouseButtonEventArgs e)
        {

        }

        public void PostprocessMouseWheel(MouseWheelEventArgs e)
        {

        }

        public void PostprocessQueryContinueDrag(QueryContinueDragEventArgs e)
        {

        }

        public void PreprocessDragEnter(DragEventArgs e)
        {

        }

        public void PreprocessDragLeave(DragEventArgs e)
        {

        }

        public void PreprocessDragOver(DragEventArgs e)
        {

        }

        public void PreprocessDrop(DragEventArgs e)
        {

        }

        public void PreprocessGiveFeedback(GiveFeedbackEventArgs e)
        {

        }

        public void PreprocessMouseDown(MouseButtonEventArgs e)
        {

        }

        public void PreprocessMouseEnter(MouseEventArgs e)
        {

        }

        public void PreprocessMouseLeave(MouseEventArgs e)
        {

        }

        public void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {

        }

        public void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {

        }

        public void PreprocessMouseMove(MouseEventArgs e)
        {

        }

        public void PreprocessMouseRightButtonDown(MouseButtonEventArgs e)
        {

        }

        public void PreprocessMouseRightButtonUp(MouseButtonEventArgs e)
        {

        }

        public void PreprocessMouseUp(MouseButtonEventArgs e)
        {

        }

        public void PreprocessMouseWheel(MouseWheelEventArgs e)
        {

        }

        public void PreprocessQueryContinueDrag(QueryContinueDragEventArgs e)
        {

        }

        #endregion
    }
}
