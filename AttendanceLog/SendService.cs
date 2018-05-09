using System.IO;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

namespace AttendanceLog
{

    public class ApiResult
    {
        public string Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }


    public class SendService
    {
        public ApiResult Send(string url, string json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            string result = string.Empty;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }


            return new ApiResult
            {
                Result = result,
                StatusCode = httpResponse.StatusCode
            };
        }
    }
}