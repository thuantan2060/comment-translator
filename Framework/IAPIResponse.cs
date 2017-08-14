using System.Collections.Generic;

namespace Framework
{
    public interface IAPIResponse
    {
        int Code { get; set; }
        string Data { get; set; }
        IDictionary<string, object> Tags { get; set; }
        string Message { get; set; }
    }
}
