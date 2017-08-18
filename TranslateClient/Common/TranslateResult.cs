namespace TranslateClient.Common
{
    public class TranslateResult
    {
        public string Text { get; set; } = "";
        public TranslateSuggest Suggest { get; set; } = new TranslateSuggest();
        public string Raw { get; set; } = "";
    }

    public class TranslateSuggest
    {
        public TranslateSuggestLanguage Language { get; set; } = new TranslateSuggestLanguage();
        public TranslateSuggestAutoCorrect Text { get; set; } = new TranslateSuggestAutoCorrect();
    }

    public class TranslateSuggestLanguage
    {
        public bool DidYouMean { get; set; } = false;
        public string Code { get; set; } = "";
    }

    public class TranslateSuggestAutoCorrect
    {
        public bool DidYouMean { get; set; } = false;
        public string Value { get; set; } = "";
        public bool AutoCorrect { get; set; } = false;
    }
}
