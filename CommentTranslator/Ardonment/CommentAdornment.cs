using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
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
        private TextBlock _tblTranslatedText;

        private CommentTranslateTag _tag;
        private SnapshotSpan _span;
        private double _lineHeight;

        private bool _isTranslating = false;
        private string _lastText;

        public CommentAdornment(CommentTranslateTag tag, SnapshotSpan span, double lineHeight)
        {
            _tag = tag;
            _span = span;
            _lineHeight = lineHeight;

            GenerateLayout(tag);
            RequestTranslate();
        }

        public void Update(CommentTranslateTag tag)
        {
            _tag = tag;

            RequestTranslate();
        }

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
            if (_tblTranslatedText != null)
            {
                _tblTranslatedText.Text = translatedText;
            }
        }

        private void GenerateLayout(CommentTranslateTag tag)
        {
            //Draw lable
            _tblTranslatedText = new TextBlock()
            {
                Foreground = Brushes.Gray
            };
            _tblTranslatedText.MouseDown += _tblTranslatedText_MouseDown;
            _tblTranslatedText.Measure(new Size(double.MaxValue, double.MaxValue));

            //Draw Line
            var line = new Line();
            line.Stroke = Brushes.LightGray;
            line.StrokeThickness = 6;
            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.X1 = 4;
            line.X2 = 4;
            line.Y1 = 4;

            switch (GetPosition(tag.Text))
            {
                case TextPosition.Bottom:
                    {
                        line.Y2 = _tblTranslatedText.DesiredSize.Height + line.Y1 - 2;

                        Canvas.SetTop(_tblTranslatedText, 4);
                        Canvas.SetLeft(_tblTranslatedText, 10);

                        this.Height = _lineHeight;
                        this.Width = 0;
                    }
                    break;
                case TextPosition.Right:
                    {
                        line.Y2 = line.Y1;

                        Canvas.SetTop(_tblTranslatedText, 0);
                        Canvas.SetLeft(_tblTranslatedText, 50);

                        this.Height = 0;
                        this.Width = 0;
                    }
                    break;
            }

            this.Children.Add(line);
            this.Children.Add(_tblTranslatedText);
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

        private void _tblTranslatedText_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //e.Handled = true;
        }
    }
}
