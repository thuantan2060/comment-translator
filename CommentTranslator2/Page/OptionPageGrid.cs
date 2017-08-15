using System;
using System.ComponentModel;
using CommentTranslator.Command;
using Microsoft.VisualStudio.Shell;

namespace CommentTranslator.Page
{
    public class OptionPageGrid : DialogPage
    {
        [Category("Server")]
        [DisplayName("Translate Server Url")]
        [Description("The url of translate server")]
        public string TranslateUrl { get; set; } = "http://mtigoogletranslateapi20170814014836.azurewebsites.net";

        [Category("Translate")]
        [DisplayName("Translate From Language")]
        [Description("The default language translate from")]
        public string TranslateFrom { get; set; } = "ja";

        [Category("Translate")]
        [DisplayName("Translate To Language")]
        [Description("The language translate to")]
        public string TranslatetTo { get; set; } = "en";

        [Category("Translate")]
        [DisplayName("Auto detect language")]
        [Description("Auto detect language translate from")]
        public bool AutoDetect { get; set; } = true;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //Reload setting
            CommentTranslatorPackage.Settings.ReloadSetting(this);
        }
    }
}
