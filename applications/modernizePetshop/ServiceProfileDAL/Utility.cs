using System;
using System.Net;

namespace ServiceProfileDAL
{
    static class Utility
    {
        public static string ServiceCall(string apiUrl, string payload)
        {
            string result = null;

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    result = client.UploadString(apiUrl, payload);
                }
            }
            catch (Exception)
            {
                //TODO: Need to log exception
                result = string.Empty;
            }
            return result;
        }
    }
}
