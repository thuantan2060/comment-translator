using System.Threading.Tasks;
using Framework;

namespace CommentTranlsator.Client
{
    public class TranslateClient : ApiClient
    {
        private string _baseUrl;

        public TranslateClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<IAPIResponse> ExecuteRequest(IApiRequest request)
        {
            return await Request(request);
        }
    }
}
