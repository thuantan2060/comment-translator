using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommentTranslator.Common;
using Framework;

namespace CommentTranlsator.Client
{
    public class TranslateClient : ApiClient
    {
        private const string CONTENT_TYPE = "application/text";
        private const string CHARSET = "UTF-8";
        private const string METHOD = "POST";

        private Settings _settings;

        public TranslateClient(Settings settings)
        {
            _settings = settings;
        }

        public async Task<IAPIResponse> Translate(string text)
        {
            var request = new ApiRequest()
            {
                ContentType = CONTENT_TYPE,
                Charset = CHARSET,
                Method = METHOD,
                Url = _settings.TranslateUrl + "/api/translate",
                Body = Encoding.UTF8.GetBytes(text),
                Headers = new Dictionary<string, string>()
            };

            request.Headers.Add("from-language", _settings.TranslateFrom);
            request.Headers.Add("to-language", _settings.TranslateTo);
            request.Headers.Add("auto-detect-language", _settings.AutoDetect.ToString());

            return await Execute(request);
        }
    }
}
