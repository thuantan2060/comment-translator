using System.Collections.Generic;
using System.Threading.Tasks;
using TranslateClient.Common;

namespace TranslateClient
{
    public interface ITranslateClient
    {
        Task<TranslateResult> Translate(string text, string sourceLang, string destinationLang, bool autoDetect = true);
        string LanguageName(string code);
    }
}
