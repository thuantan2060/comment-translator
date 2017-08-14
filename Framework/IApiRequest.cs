using System.Collections.Generic;

namespace Framework
{
    public interface IApiRequest
    {
        string Url { get; set; }
        string Method { get; set; }
        string ContentType { get; set; }
        string Charset { get; set; }
        IDictionary<string, string> Headers { get; set; }
        byte[] Body { get; set; }
    }
}
