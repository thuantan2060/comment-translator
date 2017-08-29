using Microsoft.VisualStudio.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornment : Canvas
    {
        private TextBlock _tblTranslatedText;
        private CommentTranslateTag _commentTranslateTag;
        private bool _isTranslating = false;
        private string _lastText;

        public CommentAdornment(CommentTranslateTag commentTranslateTag, SnapshotSpan span, double width, double height)
        {
            _commentTranslateTag = commentTranslateTag;

            GenerateLayout(width, height);
            RequestTranslate();
        }

        public void Update(CommentTranslateTag commentTranslateTag)
        {
            _commentTranslateTag = commentTranslateTag;
            RequestTranslate();
        }

        private void RequestTranslate()
        {
            if (!_isTranslating)
            {
                _isTranslating = true;
                WaitTranslate(_commentTranslateTag);
            }
        }

        private void WaitTranslate(CommentTranslateTag tag)
        {
            if (_commentTranslateTag.TimeWaitAfterChange <= 0)
            {
                ExecuteTranslate(tag);
            }
            else
            {
                Task.Delay(_commentTranslateTag.TimeWaitAfterChange)
                    .ContinueWith((data) =>
                    {
                        if (tag.Text == _commentTranslateTag.Text)
                        {
                            ExecuteTranslate(tag);
                        }
                        else
                        {
                            WaitTranslate(_commentTranslateTag);
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
            UpdateTranslateText("Translating...");

            Task.Run(() => CommentTranslatorPackage.TranslateClient.Translate(tag.Text))
                .ContinueWith((data) =>
                {
                    if (!data.IsFaulted)
                    {
                        UpdateTranslateText(data.Result.Data);
                    }

                    this._isTranslating = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateTranslateText(string translatedText)
        {
            if (_tblTranslatedText != null)
            {
                _tblTranslatedText.Text = translatedText;
            }
        }

        private void GenerateLayout(double width, double height)
        {
            this.Height = height;
            this.Width = width;

            _tblTranslatedText = new TextBlock()
            {
                Foreground = Brushes.Gray
            };

            Canvas.SetTop(_tblTranslatedText, 4);
            this.Children.Add(_tblTranslatedText);
        }
    }
}
