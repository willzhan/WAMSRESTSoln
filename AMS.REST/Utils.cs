using System;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;

namespace AMS.REST
{
    public class Utils
    {
        //for service principal authentication
        public static string GetUrlEncodedJWT(string clientId, string clientSecret)
        {
            string resource = System.Configuration.ConfigurationManager.AppSettings["resource"];
            string aadsts = System.Configuration.ConfigurationManager.AppSettings["aadsts"];
            string body = string.Format("resource={0}&client_id={1}&client_secret={2}&grant_type=client_credentials", HttpUtility.UrlEncode(resource), clientId, HttpUtility.UrlEncode(clientSecret));

            string jwt = null;

            HttpWebResponse response = MakeHttpRequest(aadsts, body);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader stream = new StreamReader(responseStream))
                    {
                        string responseString = stream.ReadToEnd();
                        var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(responseString), new XmlDictionaryReaderQuotas());

                        while (reader.Read())
                        {
                            if ((reader.Name == "access_token") && (reader.NodeType == XmlNodeType.Element))
                            {
                                if (reader.Read())
                                {
                                    jwt = reader.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return jwt;
        }

        //for user/interactive authentication
        public static string GetAuthorizationCode(string clientId, string clientSecret)
        {
            string resource = System.Configuration.ConfigurationManager.AppSettings["resource"];
            string aadidp = System.Configuration.ConfigurationManager.AppSettings["aadidp"];
            string redirecturi = System.Configuration.ConfigurationManager.AppSettings["redirecturi"];
            string query = string.Format("resource={0}&client_id={1}&redirect_uri={2}&response_mode=query&response_type=code&prompt=login", HttpUtility.UrlEncode(resource), clientId, HttpUtility.UrlEncode(redirecturi));
            string url = string.Format("{0}?{1}", aadidp, query);
            string authorization_code = null;

            HttpWebResponse response = MakeHttpRequest(aadidp, query);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader stream = new StreamReader(responseStream))
                    {
                        string responseString = stream.ReadToEnd();
                        var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(responseString), new XmlDictionaryReaderQuotas());

                        while (reader.Read())
                        {
                            if ((reader.Name == "code") && (reader.NodeType == XmlNodeType.Element))
                            {
                                if (reader.Read())
                                {
                                    authorization_code = reader.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return authorization_code;
        }

        public static string GetUrlEncodedAcsBearerToken(string clientId, string clientSecret)
        {
            string scope = System.Configuration.ConfigurationManager.AppSettings["OAuthScope"];
            string accessControlServiceUri = System.Configuration.ConfigurationManager.AppSettings["OAuthACSServiceUri"]; ;
            string body = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}", clientId, HttpUtility.UrlEncode(clientSecret), HttpUtility.UrlEncode(scope));

            string acsBearerToken = null;

            HttpWebResponse response = MakeHttpRequest(accessControlServiceUri, body);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader stream = new StreamReader(responseStream))
                    {
                        string responseString = stream.ReadToEnd();
                        var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(responseString), new XmlDictionaryReaderQuotas());

                        while (reader.Read())
                        {
                            if ((reader.Name == "access_token") && (reader.NodeType == XmlNodeType.Element))
                            {
                                if (reader.Read())
                                {
                                    acsBearerToken = reader.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return acsBearerToken;
        }

        //HTTP POST, body needs to be properly URL encoded. This is used by GetUrlEncodedAcsBearerToken method. Always a POST.
        public static HttpWebResponse MakeHttpRequest(string uri, string body)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(body);
            HttpWebRequest objHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            objHttpWebRequest.KeepAlive = true;
            objHttpWebRequest.ContentLength = bytes.Length;

            Stream objStream = objHttpWebRequest.GetRequestStream();
            objStream.Write(bytes, 0, bytes.Length);
            objStream.Close();

            HttpWebResponse objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();

            return objHttpWebResponse;
        }

        public static HttpWebRequest GenerateRequest(string verb,
                                                     string mediaServicesApiServerUri,
                                                     string resourcePath, 
                                                     string query,
                                                     string acsBearerToken, 
                                                     string requestbody)
        {
            var uriBuilder = new UriBuilder(mediaServicesApiServerUri);
            uriBuilder.Path += resourcePath;
            if (query != null)
            {
                uriBuilder.Query = query;
            }
            Console.WriteLine("HTTP URI: {0}", uriBuilder.Uri.PathAndQuery);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uriBuilder.Uri);
            request.AllowAutoRedirect = false; //We manage our own redirects.
            request.Method = verb;

            if (resourcePath == "$metadata")
                request.MediaType = "application/xml";
            else
            {
                request.ContentType = "application/json;odata=verbose";
                request.Accept = "application/json;odata=verbose";       
                //request.ContentType = "application/json;odata=minimalmetadata";
                //request.Accept = "application/json;odata=minimalmetadata"; 
            }

            request.Headers.Add("DataServiceVersion", "3.0");
            request.Headers.Add("MaxDataServiceVersion", "3.0");
            request.Headers.Add("x-ms-version", "2.7");                  //live starts from 2.7, before that live objects are not supported.
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + acsBearerToken);

            if (requestbody != null)
            {
                var requestBytes = Encoding.ASCII.GetBytes(requestbody);
                request.ContentLength = requestBytes.Length;

                var requestStream = request.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }
            else
            {
                request.ContentLength = 0;
            }
            return request;
        }

    }  //class Utils
} //namespace
