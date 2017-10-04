using CommentTranslator.Parsers;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CommentTranslator.Ardonment
{
    internal enum TextPositions
    {
        Bottom,
        Right
    }

    internal sealed class CommentAdornment : Canvas
    {
        #region Fields

        private TextBlock _originTextBlock;
        private TextBlock _textBlock;
        private Line _line;

        private CommentTranslateTag _tag;
        private SnapshotSpan _span;
        private IWpfTextView _view;
        private IEditorFormatMap _format;
        private ICommentParser _parser;

        private bool _isTranslating = false;
        private bool _isHide = false;
        private string _lastText;

        #endregion

        #region Contructors

        public CommentAdornment(CommentTranslateTag tag, SnapshotSpan span, IWpfTextView textView, IEditorFormatMap format)
        {
            _tag = tag;
            _span = span;
            _view = textView;
            _format = format;
            _parser = CommentParserHelper.GetCommentParser(tag.ContentType.TypeName);

            GenerateLayout(tag, textView);
            RequestTranslate(tag);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Update(CommentTranslateTag tag, SnapshotSpan span)
        {
            _tag = tag;

            RefreshLayout(tag, _view);
            RequestTranslate(tag);
        }

        #endregion

        #region Functions

        private void RequestTranslate(CommentTranslateTag tag)
        {
            //Trim comment
            var trimComment = new TrimComment() {
                OriginText = tag.Text,
                TrimedText = tag.Text,
                LineCount = CommentHelper.LineCount(tag.Text)
            };
            if (_parser != null)
            {
                trimComment = _parser.TrimComment(tag.Text);
            }

            _isHide = false;
            //Not translate empty comment
            if (string.IsNullOrEmpty(trimComment.TrimedText))
            {
                _isHide = true;
                _isTranslating = false;
                RefreshLayout(tag, _view);
            }

            //Send to translate
            if (!_isTranslating && trimComment.TrimedText != _lastText)
            {
                _isTranslating = true;
                WaitTranslate(trimComment, tag.TimeWaitAfterChange);
            }
        }

        private void WaitTranslate(TrimComment comment, int wait)
        {
            if (wait <= 0)
            {
                ExecuteTranslate(comment);
            }
            else
            {
                Task.Delay(wait)
                    .ContinueWith((data) =>
                    {
                        if (comment.OriginText == _tag.Text)
                        {
                            ExecuteTranslate(comment);
                        }
                        else
                        {
                            WaitTranslate(comment, wait);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ExecuteTranslate(TrimComment comment)
        {
            if (CommentTranslatorPackage.TranslateClient == null || _lastText == comment.TrimedText)
            {
                _isTranslating = false;
                return;
            }

            _lastText = comment.TrimedText;
            AfterTranslate(null, "Translating...");

            Task.Run(() => CommentTranslatorPackage.TranslateClient.Translate(comment.TrimedText))
                .ContinueWith((data) =>
                {
                    if (!data.IsFaulted)
                    {
                        AfterTranslate(comment, data.Result.Data);
                    }

                    this._isTranslating = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AfterTranslate(TrimComment comment, string translatedText)
        {
            if (_textBlock != null)
            {
                if (comment == null)
                {
                    _textBlock.Text = translatedText;
                }
                else
                {
                    var line = CommentHelper.LineCount(translatedText);
                    _textBlock.Text = new string('\n', comment.LineCount - line) + translatedText;
                }
            }
        }

        private void GenerateLayout(CommentTranslateTag tag, IWpfTextView textView)
        {
            //Create origin textblock
            _originTextBlock = new TextBlock();

            //Draw lable
            _textBlock = new TextBlock()
            {
                Foreground = Brushes.Gray
            };
            _textBlock.MouseDown += _tblTranslatedText_MouseDown;

            //Draw Line
            _line = new Line();
            _line.Stroke = Brushes.LightGray;
            _line.StrokeThickness = 6;
            _line.SnapsToDevicePixels = true;
            _line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            //Get format
            var typeface = _format.GetTypeface();
            var fontSize = _format.GetFontSize();
            if (typeface != null)
            {
                //Set format for origin text block
                _originTextBlock.FontFamily = typeface.FontFamily;
                _originTextBlock.FontStyle = typeface.Style;
                _originTextBlock.FontStretch = typeface.Stretch;
                _originTextBlock.FontWeight = typeface.Weight;
                _originTextBlock.FontSize = fontSize;

                //Set format for text block
                _textBlock.FontFamily = typeface.FontFamily;
                _textBlock.FontStyle = typeface.Style;
                _textBlock.FontStretch = typeface.Stretch;
                _textBlock.FontWeight = typeface.Weight;
                _textBlock.FontSize = fontSize;
            }

            //Refresh layout
            RefreshLayout(tag, textView);

            this.Children.Add(_line);
            this.Children.Add(_textBlock);
        }

        private void RefreshLayout(CommentTranslateTag tag, IWpfTextView textView)
        {
            if (_isHide)
            {
                this.Width = 0;
                this.Height = 0;
                this.Visibility = Visibility.Collapsed;
                return;
            }

            this.Visibility = Visibility.Visible;

            //Set text
            _originTextBlock.Text = _parser.SimpleTrimComment(tag.Text);

            //Measure size
            _originTextBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            _textBlock.Measure(new Size(double.MaxValue, double.MaxValue));

            //Get position
            switch (GetPosition(tag.Text))
            {
                case TextPositions.Bottom:
                    RefreshLayoutBottom(tag, textView);
                    break;
                case TextPositions.Right:
                    RefreshLayoutRight(tag, textView);
                    break;
            }
        }

        private void RefreshLayoutBottom(CommentTranslateTag tag, IWpfTextView textView)
        {
            //Set line position
            _line.X1 = _line.X2 = 4;
            _line.Y1 = 4;
            _line.Y2 = _originTextBlock.DesiredSize.Height + _line.Y1 - 2;

            //Set text box position
            Canvas.SetTop(_textBlock, 4);
            Canvas.SetLeft(_textBlock, 10);

            //Set size of canvas
            this.Height = textView.GetLineHeight();
            this.Width = 0;
        }

        private void RefreshLayoutRight(CommentTranslateTag tag, IWpfTextView textView)
        {
            //Calculate top left position
            var top = -textView.GetLineHeight() + 4;
            var left = _originTextBlock.DesiredSize.Width + 10;

            //Set position of text box
            Canvas.SetTop(_textBlock, top);
            Canvas.SetLeft(_textBlock, left);

            //Set position of line
            _line.X1 = _line.X2 = left - 6;
            _line.Y1 = top;
            _line.Y2 = Math.Max(_textBlock.DesiredSize.Height, _originTextBlock.DesiredSize.Height) + _line.Y1 - 2;

            //Set size of canvas
            this.Height = 0;
            this.Width = 0;
        }

        private TextPositions GetPosition(string text)
        {
            if (text.IndexOf('\n') > 0)
            {
                //if (_originTextBlock.DesiredSize.Width > 400)
                //{
                //    return TextPositions.Bottom;
                //}
                //else
                //{
                //    return TextPositions.Right;
                //}
                return TextPositions.Right;
            }
            else
            {
                return TextPositions.Bottom;
            }
        }

        #endregion

        #region Events

        #endregion

        #region EventHandlers

        private void _tblTranslatedText_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //e.Handled = true;
        }

        #endregion

        #region InnerMembers

        #endregion
    }
}
