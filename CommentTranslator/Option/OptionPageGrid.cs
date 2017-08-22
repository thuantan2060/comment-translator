using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace CommentTranslator.Option
{
    public class OptionPageGrid : DialogPage
    {
        [Category("Server")]
        [DisplayName("Translate Server Url")]
        [Description("The url of translate server")]
        public string TranslateUrl { get; set; } = "http://mti-translate-api-mti-translate-api.a3c1.starter-us-west-1.openshiftapps.com/";

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
