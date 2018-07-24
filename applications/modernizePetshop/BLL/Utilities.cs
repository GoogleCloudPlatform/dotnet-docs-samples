using System;
using System.Net;

namespace PetShop.BLL
{
    static class Utilities
    {
        public static string WebAPICall(string apiUrl)
        {
            string result = null;

            try
            {
                using (var client = new WebClient())
                {
                    result = client.DownloadString(apiUrl);
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }
        public static string WebAPICallWithData(string apiUrl, string payload)
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
