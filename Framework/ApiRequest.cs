using System.Collections.Generic;

namespace Framework
{
    public class ApiRequest : IApiRequest
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public string Charset { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public byte[] Body { get; set; }
    }
}
