using CommentTranslator.Support;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CommentTranslator.Ardonment
{
    internal enum TextPosition
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

        private bool _isTranslating = false;
        private string _lastText;

        #endregion

        #region Contructors

        public CommentAdornment(CommentTranslateTag tag, SnapshotSpan span, IWpfTextView textView)
        {
            _tag = tag;
            _span = span;
            _view = textView;

            GenerateLayout(tag, textView);
            RefreshLayout(tag, textView);
            RequestTranslate();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Update(CommentTranslateTag tag, SnapshotSpan span)
        {
            _tag = tag;

            RequestTranslate();
        }

        #endregion

        #region Functions

        private void RequestTranslate()
        {
            if (!_isTranslating && _tag.Text != _lastText)
            {
                _isTranslating = true;
                WaitTranslate(_tag);
            }
        }

        private void WaitTranslate(CommentTranslateTag tag)
        {
            if (_tag.TimeWaitAfterChange <= 0)
            {
                ExecuteTranslate(tag);
            }
            else
            {
                Task.Delay(_tag.TimeWaitAfterChange)
                    .ContinueWith((data) =>
                    {
                        if (tag.Text == _tag.Text)
                        {
                            ExecuteTranslate(tag);
                        }
                        else
                        {
                            WaitTranslate(_tag);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ExecuteTranslate(CommentTranslateTag tag)
        {
            if (CommentTranslatorPackage.TranslateClient == null) return;
            if (_lastText == tag.Text)
            {
                _isTranslating = false;
                return;
            }

            _lastText = tag.Text;
            AfterTranslate("Translating...");

            Task.Run(() => CommentTranslatorPackage.TranslateClient.Translate(CommentHelper.TrimCommnentSingleline(tag.Text)))
                .ContinueWith((data) =>
                {
                    if (!data.IsFaulted)
                    {
                        AfterTranslate(data.Result.Data);
                    }

                    this._isTranslating = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AfterTranslate(string translatedText)
        {
            if (_textBlock != null)
            {
                _textBlock.Text = translatedText;
            }
        }

        private void GenerateLayout(CommentTranslateTag tag, IWpfTextView textView)
        {
            //Create origin textblock
            _originTextBlock = new TextBlock()
            {
                Text = tag.Text,
                FontSize = 12.5
            };
            _originTextBlock.Measure(new Size(double.MaxValue, double.MaxValue));

            //Draw lable
            _textBlock = new TextBlock()
            {
                Foreground = Brushes.Gray,
                FontSize = 12.5
            };
            _textBlock.MouseDown += _tblTranslatedText_MouseDown;
            _textBlock.Measure(new Size(double.MaxValue, double.MaxValue));

            //Draw Line
            _line = new Line();
            _line.Stroke = Brushes.LightGray;
            _line.StrokeThickness = 6;
            _line.SnapsToDevicePixels = true;
            _line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            _line.X1 = 4;
            _line.X2 = 4;
            _line.Y1 = 4;
            _line.Y2 = _originTextBlock.DesiredSize.Height + _line.Y1 - 2;

            this.Width = 0;
            this.Height = 0;

            this.Children.Add(_line);
            this.Children.Add(_textBlock);
        }

        private void RefreshLayout(CommentTranslateTag tag, IWpfTextView textView)
        {
            _textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            switch (GetPosition(tag.Text))
            {
                case TextPosition.Bottom:
                    {
                        Canvas.SetTop(_textBlock, 4);
                        Canvas.SetLeft(_textBlock, 10);

                        this.Height = textView.GetLineHeight();
                    }
                    break;
                case TextPosition.Right:
                    {
                        var top = -textView.GetLineHeight() + 4;
                        var left = _originTextBlock.DesiredSize.Width + 4;

                        Canvas.SetTop(_textBlock, top);
                        Canvas.SetLeft(_textBlock, left);

                        _line.X1 = _line.X2 = left;
                        _line.Y1 = top - 6;

                        this.Height = 0;
                    }
                    break;
            }

            _line.Y2 = Math.Max(_textBlock.DesiredSize.Height, _originTextBlock.DesiredSize.Height) + _line.Y1 - 2;
            this.Width = 0;
        }

        private TextPosition GetPosition(string text)
        {
            var firstNewLine = text.IndexOf('\n');
            var lastNewLine = text.LastIndexOf('\n');

            if (firstNewLine > 0 && firstNewLine != lastNewLine)
            {
                return TextPosition.Right;
            }
            else
            {
                return TextPosition.Bottom;
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
