using CommentTranslator.Parsers;
using CommentTranslator.Support;
using CommentTranslator.Util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CommentTranslator.Ardonment
{
    internal sealed class CommentAdornment : Canvas, IAdornment
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

        private int _id = new Random().Next();
        private bool _isTranslating;
        private CommentTranslateTag _translateTag;
        private Comment _translatedComment;

        #endregion

        #region Contructors

        public CommentAdornment(CommentTranslateTag tag, SnapshotSpan span, IWpfTextView textView, IEditorFormatMap format, SnapshotSpan originSpan)
        {
            _tag = tag;
            _span = span;
            _view = textView;
            _format = format;
            _parser = CommentParserHelper.GetCommentParser(tag.ContentType.TypeName);

            Span = originSpan;

            GenerateLayout(tag);
            Translate(tag);
        }

        #endregion

        #region Properties

        public SnapshotSpan Span { get; private set; }

        #endregion

        #region Methods

        public void Update(CommentTranslateTag tag, SnapshotSpan span, SnapshotSpan originSpan)
        {
            if (tag.Text != _tag.Text)
            {
                //Refresh layout
                RefreshLayout(_parser.GetComment(tag));

                //Request translate
                Translate(tag);
            }

            //Set properties
            _tag = tag;
            _span = span;
            Span = originSpan;
        }

        #endregion

        #region Functions

        private void GenerateLayout(CommentTranslateTag tag)
        {
            //Create origin textblock
            _originTextBlock = new TextBlock();

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
            RefreshLayout(_parser.GetComment(tag));

            //Add to parent
            this.Children.Add(_line);
            this.Children.Add(_textBlock);
        }

        private void RefreshLayout(Comment comment, bool hideOnEmpty = true)
        {
            //Hide on empty
            if (hideOnEmpty && string.IsNullOrEmpty(comment.Trimmed))
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }

            //Set text
            _originTextBlock.Text = _parser.SimpleTrimComment(comment.Origin);

            //Measure size
            _originTextBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            _textBlock.Measure(new Size(double.MaxValue, double.MaxValue));

            switch (comment.Position)
            {
                case TextPositions.Bottom:
                    RefreshLayoutBottom(comment);
                    break;
                case TextPositions.Right:
                    RefreshLayoutRight(comment);
                    break;
            }
        }

        private void RefreshLayoutBottom(Comment comment)
        {
            //Set line position
            _line.X1 = _line.X2 = 4;
            _line.Y1 = 4;
            _line.Y2 = _originTextBlock.DesiredSize.Height + _line.Y1 - 2;

            //Set text box position
            Canvas.SetTop(_textBlock, 4);
            Canvas.SetLeft(_textBlock, 10);

            //Set size of canvas
            this.Height = _view.GetLineHeight();
            this.Width = 0;
        }

        private void RefreshLayoutRight(Comment comment)
        {
            //Calculate top left position
            var top = -_view.GetLineHeight() + 5;
            var left = _originTextBlock.DesiredSize.Width + 20;

            //Set position of text box
            Canvas.SetTop(_textBlock, top);
            Canvas.SetLeft(_textBlock, left);

            //Set position of line
            _line.X1 = _line.X2 = left - 5;
            _line.Y1 = top - 1;
            _line.Y2 = Math.Max(_textBlock.DesiredSize.Height, _originTextBlock.DesiredSize.Height) + _line.Y1 + 1;

            //Set size of canvas
            this.Height = 0;
            this.Width = 0;
        }

        private Comment ConvertToComment(CommentTranslateTag tag)
        {
            if (_parser != null)
            {
                return _parser.GetComment(tag);
            }

            return new Comment()
            {
                Origin = tag.Text,
                Trimmed = tag.Text,
                Line = CommentHelper.LineCount(tag.Text),
                Position = TextPositions.Bottom
            };
        }

        private bool IsHide(Comment comment)
        {
            //Not translate empty comment
            if (string.IsNullOrEmpty(comment.Trimmed))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Translate functions

        private void Translate(CommentTranslateTag tag, bool force = false)
        {
            //Simple trim text
            var trimmedText = _parser.SimpleTrimComment(tag.Text);

            //Set translating tag
            _translateTag = tag;

            if ((force || !_isTranslating) && !string.IsNullOrEmpty(trimmedText))
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
                            if (trimmedText != _parser.SimpleTrimComment(_translateTag.Text))
                            {
                                Translate(_translateTag, true);
                            }
                            else
                            {
                                _isTranslating = false;
                                StartTranslate(tag);
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void StartTranslate(CommentTranslateTag tag)
        {
            var comment = _parser.GetComment(tag);
            if (!string.IsNullOrEmpty(comment.Trimmed) && (_translatedComment == null || comment.Trimmed != _translatedComment.Trimmed))
            {
                //Set translated comment
                _translatedComment = comment;

                //Display wait translate
                WaitTranslate("Translating...");

                //Translate comment
                Task
                    .Run(() => CommentTranslatorPackage.TranslateClient.Translate(comment.Trimmed))
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

        #endregion
    }

    internal class TranslatedComment : Comment
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
            this.Trimmed = comment.Trimmed;
            this.MarginTop = comment.MarginTop;
            this.Tag = comment.Tag;
            this.Translated = translated;
        }

        public string Translated { get; set; }
    }
}
