using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for FirebaseMembership
/// </summary>
public static class FirebaseMembership
{
    private static readonly string apiUrl = "associate";
    public static void AssociateFirebase(string username, string firebaseGUID)
    {
        string baseURL = ConfigurationManager.AppSettings["ProfileBaseURL"];
        string result = null;
        try
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                result = client.UploadString(baseURL + apiUrl, JsonConvert.SerializeObject(new User(username, firebaseGUID)));
            }
        }
        catch (Exception)
        {
            result = string.Empty;
        }
    }
    public class User
    {
        public string Username;
        public string FirebaseGUID;
        public User(string username, string guid)
        {
            Username = username;
            FirebaseGUID = guid;
        }
    }
}