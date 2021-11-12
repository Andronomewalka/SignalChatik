using System.Net;

namespace SignalChatik.DTO
{
    public class JsonResponseBodyGood<T> 
    {
        public T Data { get; set; }
    }

    public class JsonResponseBodyBad
    {
        public int Code { get; set; }
        public string Error { get; set; }
    }
}
