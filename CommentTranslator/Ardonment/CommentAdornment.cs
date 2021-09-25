using CommentTranslator.Parsers;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornment : Canvas
    {
        #region Fields

        private Typeface _typeface;
        private double _fontSize;
        private TextBlock _textBlock;
        private Line _line;

        private CommentTag _tag;
        private SnapshotSpan _span;
        private SnapshotSpan _containSpan;
        private IWpfTextView _view;
        private IEditorFormatMap _format;

        private bool _isTranslating;
        private CommentTag _currentTag;
        private Comment _translatedComment;

        #endregion

        #region Contructors

        public CommentAdornment(CommentTag tag, SnapshotSpan span, IWpfTextView textView, IEditorFormatMap format, SnapshotSpan containSpan)
        {
            _tag = tag;
            _span = span;
            _view = textView;
            _format = format;
            _containSpan = containSpan;

            GenerateLayout(tag);
            Translate(tag);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Update(CommentTag tag, SnapshotSpan span, SnapshotSpan containSpan)
        {
            //Set properties
            _span = span;
            _containSpan = containSpan;

            if (tag.Comment.Content != _tag.Comment.Content || tag.Comment.Line != _tag.Comment.Line)
            {
                //Refresh layout
                RefreshLayout(tag.Comment);

                //Request translate
                Translate(tag);
            }

            //Set properties
            _tag = tag;
        }

        #endregion

        #region Functions

        private void GenerateLayout(CommentTag tag)
        {
            //Draw lable
            _textBlock = new TextBlock()
            {
                Foreground = Brushes.Gray
            };

            //Draw Line
            _line = new Line();
            _line.Stroke = Brushes.LightGray;
            _line.StrokeThickness = 6;
            _line.SnapsToDevicePixels = true;
            _line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            //Get format
            _typeface = _format.GetTypeface();
            _fontSize = _format.GetFontSize();
            if (_typeface != null)
            {
                //Set format for text block
                _textBlock.FontFamily = _typeface.FontFamily;
                _textBlock.FontStyle = _typeface.Style;
                _textBlock.FontStretch = _typeface.Stretch;
                _textBlock.FontWeight = _typeface.Weight;
                _textBlock.FontSize = _fontSize;
            }

            //Refresh layout
            RefreshLayout(tag.Comment);

            //Add to parent
            this.Children.Add(_line);
            this.Children.Add(_textBlock);
        }

        private void RefreshLayout(Comment comment, bool hideOnEmpty = true)
        {
            //Hide on empty
            if (hideOnEmpty && string.IsNullOrEmpty(comment.Content))
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }

            //Measure size
            var format = MeasureFormat(comment.Origin);

            //Refresh for specify layout
            switch (comment.Position)
            {
                case TextPositions.Bottom:
                    RefreshLayoutBottom(comment, format);
                    break;
                case TextPositions.Right:
                    RefreshLayoutRight(comment, format);
                    break;
            }
        }

        private void RefreshLayoutBottom(Comment comment, FormattedText format)
        {
            //Set line position
            _line.X1 = _line.X2 = 4;
            _line.Y1 = 4;
            _line.Y2 = format.Height + _line.Y1;

            //Set text box position
            Canvas.SetTop(_textBlock, 4);
            Canvas.SetLeft(_textBlock, 10);

            //Set size of canvas
            this.Height = format.Height;
            this.Width = 0;
        }

        private void RefreshLayoutRight(Comment comment, FormattedText format)
        {
            //Calculate top left position
            var top = -GetLineHeight(_view) + 5;
            var left = format.Width + 20;

            //Set position of text box
            Canvas.SetTop(_textBlock, top);
            Canvas.SetLeft(_textBlock, left);

            //Set position of line
            _line.X1 = _line.X2 = left - 5;
            _line.Y1 = top - 1;
            _line.Y2 = format.Height + _line.Y1;

            //Set size of canvas
            this.Height = 0;
            this.Width = 0;
        }

        private FormattedText MeasureFormat(string candidate)
        {
            if (_typeface != null)
            {
                return new FormattedText(
                                        candidate,
                                        CultureInfo.CurrentUICulture,
                                        FlowDirection.LeftToRight,
                                        _typeface,
                                        _fontSize,
                                        Brushes.Black);
            }
            else
            {
                return new FormattedText(
                                       candidate,
                                       CultureInfo.CurrentUICulture,
                                       FlowDirection.LeftToRight,
                                       new Typeface(_textBlock.FontFamily, _textBlock.FontStyle, _textBlock.FontWeight, _textBlock.FontStretch),
                                       _textBlock.FontSize,
                                       Brushes.Black);
            }
        }

        private static double GetLineHeight(IWpfTextView view)
        {
            var height = 20d;
            try
            {
                height = view.LineHeight;
            }
            catch { }

            return height;
        }

        #endregion

        #region Translate functions

        private void Translate(CommentTag tag, bool force = false)
        {
            //Set translating tag
            _currentTag = tag;

            if (force || !_isTranslating)
            {
                //Set translating
                _isTranslating = true;

                //Wait to translate
                if (tag.TimeWaitAfterChange <= 0)
                {
                    _isTranslating = false;
                    StartTranslate(tag);
                }
                else
                {
                    Task.Delay(tag.TimeWaitAfterChange)
                        .ContinueWith((data) =>
                        {
                            if (!data.IsFaulted)
                            {
                                if (tag.Comment.Content != _currentTag.Comment.Content)
                                {
                                    Translate(_currentTag, true);
                                }
                                else
                                {
                                    _isTranslating = false;
                                    StartTranslate(tag);
                                }
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void StartTranslate(CommentTag tag)
        {
            var comment = tag.Comment;
            if (!string.IsNullOrEmpty(comment.Content) && (_translatedComment == null || comment.Content != _translatedComment.Content))
            {
                //Set translated comment
                _translatedComment = comment;

                //Display wait translate
                WaitTranslate("Translating...");

                //Translate comment
                Task
                    .Run(() => CommentTranslatorPackage.TranslateClient.Translate(comment.Content))
                    .ContinueWith((data) =>
                    {
                        //Call translate complete
                        if (!data.IsFaulted)
                        {
                            TranslateComplete(new TranslatedComment(comment, data.Result.Data), null);
                        }
                        else
                        {
                            TranslateComplete(new TranslatedComment(comment, data.Result.Data), data.Exception);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void TranslateComplete(TranslatedComment comment, Exception error)
        {
            if (error != null)
            {
                _textBlock.Foreground = Brushes.Red;
                _textBlock.Text = error.Message;
            }
            else
            {
                _textBlock.Foreground = Brushes.Gray;
                _textBlock.Text = new string('\n', comment.MarginTop) + comment.Translated;
            }
        }

        private void WaitTranslate(string waitingText)
        {
            _textBlock.Foreground = Brushes.Gray;
            _textBlock.Text = waitingText;
        }

        #endregion

        #region Events

        #endregion

        #region EventHandlers

        #endregion

        #region InnerMembers

        private class TranslatedComment : Comment
        {
            public TranslatedComment(string origin, TextPositions position)
            {
                this.Origin = origin;
                this.Position = position;
            }

            public TranslatedComment(Comment comment, string translated)
            {
                this.Line = comment.Line;
                this.Origin = comment.Origin;
                this.Position = comment.Position;
                this.Content = comment.Content;
                this.MarginTop = comment.MarginTop;
                this.Translated = translated;
            }

            public string Translated { get; set; }
        }

        #endregion
    }
}
