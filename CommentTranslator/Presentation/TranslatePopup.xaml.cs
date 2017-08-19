using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;

namespace CommentTranslator.Presentation
{
    /// <summary>
    /// Interaction logic for TranslatePopup.xaml
    /// </summary>
    public partial class TranslatePopup : UserControl
    {
        #region Fields

        private bool _isClose = false;

        #endregion

        #region Contructors

        public TranslatePopup(SnapshotSpan span, string text, Size viewportSize)
        {
            Span = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
            Text = text;

            InitializeComponent();

        }

        #endregion

        #region Properties

        public ITrackingSpan Span { get; set; }

        public string Text { get; set; }

        #endregion

        #region Fuctions

        private void SetMaxSize(Size viewportSize)
        {
            var maxHeight = viewportSize.Height / 2.0d;
            if (maxHeight > 600)
                maxHeight = 600;
            if (maxHeight < 150)
                maxHeight = 150;
            tblTranslatedText.MaxHeight = maxHeight;
        }

        private void Translate(string text)
        {
            tblError.Visibility = Visibility.Collapsed;
            bdTranslatedText.Visibility = Visibility.Collapsed;
            tblDirection.Text = "Translating...";

            Task
                .Run(() => CommentTranslatorPackage.TranslateClient.Translate(text))
                .ContinueWith((data) =>
                {
                    if (!_isClose)
                    {
                        if (!data.IsFaulted)
                        {
                            if (data.Result.Code == 200 && (bool)data.Result.Tags["translate-success"])
                            {
                                tblDirection.Text = string.Format("{0} -> {1}", data.Result.Tags["from-language"].ToString().ToUpper(), data.Result.Tags["to-language"].ToString().ToUpper());
                                tblTranslatedText.Text = data.Result.Data;
                                bdTranslatedText.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                tblDirection.Text = "Translate Error";
                                tblError.Text = data.Result.Message;
                                tblError.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            tblDirection.Text = "Translate Error";
                            tblError.Text = data.Exception.Message;
                            tblError.Visibility = Visibility.Visible;
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region Methods

        public void Translate()
        {
            Translate(Text);
        }

        public void Close()
        {
            _isClose = true;
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        public event EventHandler OnClosed;

        #endregion

        #region EventHandlers

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            Translate(Text);
        }

        private void CmdClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
