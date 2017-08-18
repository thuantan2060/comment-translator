using System.Collections.Generic;
using Framework;
using TranslateClient.Common;
using System.Text;
using System.Net;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Noesis.Javascript;

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
            

            

            return new GoogleToken("");
        }

        private void AttachParseTkk(JavascriptContext context)
        {
            context.Run(@"
                function sM(a) {
                    var b;
                    if (null !== yr)
                        b = yr;
                    else {
                        b = wr(String.fromCharCode(84));
                        var c = wr(String.fromCharCode(75));
                        b = [b(), b()];
                        b[1] = c();
                        b = (yr = window[b.join(c())] || '') || ''
                    }
                    var d = wr(String.fromCharCode(116))
                        , c = wr(String.fromCharCode(107))
                        , d = [d(), d()];
                    d[1] = c();
                    c = '&' + d.join('') + '=';
                    d = b.split('.');
                    b = Number(d[0]) || 0;
                    for (var e = [], f = 0, g = 0; g < a.length; g++) {
                        var l = a.charCodeAt(g);
                        128 > l ? e[f++] = l : (2048 > l ? e[f++] = l >> 6 | 192 : (55296 == (l & 64512) && g + 1 < a.length && 56320 == (a.charCodeAt(g + 1) & 64512) ? (l = 65536 + ((l & 1023) << 10) + (a.charCodeAt(++g) & 1023),
                            e[f++] = l >> 18 | 240,
                            e[f++] = l >> 12 & 63 | 128) : e[f++] = l >> 12 | 224,
                            e[f++] = l >> 6 & 63 | 128),
                            e[f++] = l & 63 | 128)
                    }
                    a = b;
                    for (f = 0; f < e.length; f++)
                        a += e[f],
                            a = xr(a, '+-a^+6');
                    a = xr(a, '+-3^+b+-f');
                    a ^= Number(d[1]) || 0;
                    0 > a && (a = (a & 2147483647) + 2147483648);
                    a %= 1E6;
                    return c + (a.toString() + '.' + (a ^ b))
                }

                var yr = null;
                var wr = function(a) {
                    return function() {
                        return a
                    }
                }
                    , xr = function(a, b) {
                    for (var c = 0; c < b.length - 2; c += 3) {
                        var d = b.charAt(c + 2)
                            , d = 'a' <= d ? d.charCodeAt(0) - 87 : Number(d)
                            , d = '+' == b.charAt(c + 1) ? a >>> d : a << d;
                        a = '+' == b.charAt(c) ? a + d & 4294967295 : a ^ d
                    }
                    return a
                };
            ");
        }

        private void AttachTkk(JavascriptContext context, string tkk)
        {
            context.Run(string.Format(@"
                var window = {
                    TKK: '{0}'
                };
            ", tkk));
        }

        private string GetTokenFromHtml(string htmlContent)
        {
            //Find token code
            var match = TOKEN_REGEX.Match(htmlContent);
            if (match == null || !match.Success)
            {
                throw new Exception("Cannot get google translate token");
            }

            //Get javascript get generate token code 
            var getTokenFunctionJs = match.Value;

            //Run javascript to get raw token
            var jsContext = new JavascriptContext();
            jsContext.Run(getTokenFunctionJs);
            var tkk = jsContext.GetParameter("TKK") as string;

            //Attach js funtion
            AttachTkk(jsContext, tkk);
            AttachParseTkk(jsContext);

            //Get token
            var token = jsContext.Run(@"sM(text)")
        }
    }
}
