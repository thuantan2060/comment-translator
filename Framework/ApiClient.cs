using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Framework
{
    public abstract class ApiClient
    {
        protected virtual async Task<IAPIResponse> Execute(IApiRequest request)
        {
            //Create empty result
            var apiResult = new ApiResponse();

            //Create http request content
            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), request.Url);

            //Assign http content
            httpRequest.Content = new ByteArrayContent(request.Body);

            //Add header param
            foreach (var param in request.Headers.Keys)
            {
                httpRequest.Headers.TryAddWithoutValidation(param, request.Headers[param]);
            }

            //Add Content type and charset
            httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
            httpRequest.Content.Headers.ContentType.CharSet = request.Charset;


            //Send Request
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.SendAsync(httpRequest);
                    var responseText = await response.Content.ReadAsStringAsync();


                    if (response.Headers.TryGetValues("from-language", out IEnumerable<string> fromLanguages))
                    {
                        apiResult.Tags.Add("from-language", fromLanguages.FirstOrDefault());
                    }
                    if (response.Headers.TryGetValues("to-language", out IEnumerable<string> toLanguages))
                    {
                        apiResult.Tags.Add("to-language", toLanguages.FirstOrDefault());
                    }

                    if (response.Headers.TryGetValues("translate-success", out IEnumerable<string> translateSuccesses))
                    {
                        apiResult.Tags.Add("translate-success", translateSuccesses.FirstOrDefault() == "true");
                    }

                    apiResult.Code = (int)response.StatusCode;
                    apiResult.Message = response.StatusCode.ToString();
                    apiResult.Data = responseText;
                }
                catch (Exception e)
                {
                    apiResult.Code = -1;
                    apiResult.Message = e.Message;
                    apiResult.Data = "";
                }
            }

            return apiResult;
        }
    }
}
