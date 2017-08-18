using System.Collections.Generic;
using Framework;
using TranslateClient.Common;
using System.Text;
using System.Net;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace TranslateClient.Google
{
    public class GoogleTranslateClient : ApiClient, ITranslateClient
    {
        #region Singleton

        private static ITranslateClient _client;

        public static ITranslateClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new GoogleTranslateClient("https://translate.google.com/translate_a/single", "https://translate.google.com");
                }

                return _client;
            }
        }

        #endregion

        private const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        private const string CHARSET = "utf-8";
        private const string METHOD = "POST";

        private readonly Regex TOKEN_REGEX = new Regex(@"TKK=(.*)\(\)\)'\);", RegexOptions.IgnoreCase);

        private static GoogleToken _token;

        private string _baseUrl;
        private string _tokenUrl;
        private GoogleLanguageMap _sourceLanguages = new GoogleLanguageMap();
        private GoogleLanguageMap _targetLanguages = new GoogleLanguageMap();

        public GoogleTranslateClient(string baseUrl, string tokenUrl)
        {
            _baseUrl = baseUrl;
            _tokenUrl = tokenUrl;
        }

        public async Task<TranslateResult> Translate(string text, string sourceLang, string destinationLang, bool autoDetect = true)
        {
            //Create Request
            var request = CreateTranslateRequest(text, sourceLang, destinationLang, autoDetect);

            //Execute request
            var response = await Execute(request);

            //Check response
            if (response == null)
            {
                throw new Exception("Server not response");
            }

            if (response.Code != 200)
            {
                throw new Exception("Translate Error: " + response.Message);
            }

            //Parse request
            return ParseTranslateResponse(response.Data);
        }

        public string LanguageName(string code)
        {
            throw new NotImplementedException();
        }

        private string CreateQuerystring(Dictionary<string, object> args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in args.Keys)
            {
                sb.Append(WebUtility.UrlEncode(name));
                sb.Append("=");
                sb.Append(WebUtility.UrlEncode(args[name].ToString()));
                sb.Append("&");
            }
            return sb.ToString(0, Math.Max(sb.Length - 1, 0));
        }

        private IApiRequest CreateTranslateRequest(string text, string sourceLang, string destinationLang, bool autoDetect)
        {
            //Create request body
            var requestBody = CreateQuerystring(new Dictionary<string, object>()
            {
                { "client", "t"},
                {"sl", sourceLang },
                {"tl", destinationLang },
                {"hl", destinationLang },
                {"dt", new string[] {"at", "bd", "ex", "ld", "md", "qca", "rw", "rm", "ss", "t" } },
                {"ie", "UTF-8" },
                {"oe", "UTF-8"},
                {"otf", 1},
                {"ssel", 0},
                {"tsel", 0},
                {"kc", 7},
                {"q", text }
            });

            //Create request
            return new ApiRequest()
            {
                ContentType = CONTENT_TYPE,
                Charset = CHARSET,
                Method = METHOD,
                Url = _baseUrl,
                Headers = new Dictionary<string, string>()
                {
                    {
                        "user-agent",
                        "User-Agent: Mozilla/5.0"
                    }
                },
                Body = Encoding.UTF8.GetBytes(requestBody)
            };
        }

        private TranslateResult ParseTranslateResponse(string response)
        {
            var translateResult = new TranslateResult();
            var jResult = JArray.Parse(response);



            return translateResult;
        }

        private async Task<GoogleToken> GetToken()
        {
            var response = await Execute(new ApiRequest()
            {
                Method = "GET",
                Url = _tokenUrl
            });

            //Check response
            if (response == null || response.Code != 200)
            {
                throw new Exception("Token server not response");
            }

            //Get html content
            var htmlContent = response.Data;
            var match = TOKEN_REGEX.Match(htmlContent);

            if (match == null || !match.Success)
            {
                throw new Exception("Cannot get google translate token");
            }

            //Get javascript get token code 
            var getTokenJs = match.Value;

            //TODO run javascript
            return new GoogleToken("");
        }
    }
}
