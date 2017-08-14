using System.Collections.Generic;

namespace Framework
{
    public class ApiResponse : IAPIResponse
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public IDictionary<string, object> Tags { get; set; }
        public string Message { get; set; }
    }
}
