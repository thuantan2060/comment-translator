using CommentTranslator.Option;

namespace CommentTranslator.Util
{
    public class Settings
    {
        //public const string SettingCollection = @"InstalledProducts\Comment Translator";

        //public const string TranslateUrlProperty = @"TranslateUrl";
        //public const string TranslateFromProperty = @"TranslateFrom";
        //public const string TranslateToProperty = @"TranslateTo";
        //public const string AutoDetectProperty = @"AutoDetect";

        public string TranslateUrl { get; set; }
        public string TranslateFrom { get; set; }
        public string TranslateTo { get; set; }
        public bool AutoDetect { get; set; }
        public bool AutoTranslateComment { get; set; }

        public void ReloadSetting(OptionPageGrid page)
        {
            this.TranslateUrl = page.TranslateUrl;
            this.TranslateFrom = page.TranslateFrom;
            this.TranslateTo = page.TranslatetTo;
            this.AutoDetect = page.AutoDetect;
            this.AutoTranslateComment = page.AutoTranslateComment;
        }
    }
}
