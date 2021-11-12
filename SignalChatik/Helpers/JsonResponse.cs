using Microsoft.AspNetCore.Mvc;
using SignalChatik.DTO;
using System.Net;

namespace SignalChatik.Helpers
{
    public static class JsonResponse
    {
        public static JsonResult CreateGood<T>(T data)
        {
            return new JsonResult(data);
        }

        public static JsonResult CreateBad(int code, string error = "")
        {
            return new JsonResult(new JsonResponseBodyBad()
            {
                Code = code,
                Error = error
            });
        }

        public static JsonResult CreateBad(HttpStatusCode code, string error = "")
        {
            return new JsonResult(new JsonResponseBodyBad()
            {
                Code = (int)code,
                Error = error
            });
        }
    }
}
