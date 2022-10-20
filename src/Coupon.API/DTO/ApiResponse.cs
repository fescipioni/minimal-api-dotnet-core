using System.Net;

namespace Coupon.API.DTO
{
    public class ApiResponse
    { 
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 300;
        public object Result { get; set; }
        public IList<string> Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }
    }
}
