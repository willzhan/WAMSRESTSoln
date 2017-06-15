using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Json;

namespace AMS.REST
{
    public class AMSClient
    {
        //make JWT token explicit input since there are scenarios you need to use more than one media services at the same time.
        public static XmlDocument MakeRestCall(string verb, string jwt, string restapirui, string resourcePath, string requestBody, bool autoRedirect = true, string query=null)
        {
            XmlDocument objXmlDocument = null;
            HttpWebResponse objHttpWebResponse = null;
            try
            {
                var request = Utils.GenerateRequest(verb, restapirui, resourcePath, query, jwt, requestBody);
                objHttpWebResponse = (HttpWebResponse)request.GetResponse();

                switch (objHttpWebResponse.StatusCode)
                {
                    case HttpStatusCode.MovedPermanently:
                        //Recurse once with the mediaServicesApiServerUri redirect Location:
                        if (autoRedirect)
                        {
                            string redirectedMediaServicesApiServerUri = objHttpWebResponse.Headers["Location"];
                            //Console.WriteLine("Redirect URL = {0}", redirectedMediaServicesApiServerUri);
                            objXmlDocument = MakeRestCall(verb, jwt, redirectedMediaServicesApiServerUri, resourcePath, requestBody, false, query: query);
                        }
                        else
                        {
                            Console.WriteLine("Redirection to {0} failed.", restapirui);
                            return null;
                        }
                        break;
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                        using (Stream responseStream = objHttpWebResponse.GetResponseStream())
                        {
                            using (StreamReader stream = new StreamReader(responseStream))
                            {
                                string responseString = stream.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseString))
                                {
                                    XmlDictionaryReader objXmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(responseString), new XmlDictionaryReaderQuotas());

                                    objXmlDocument = new XmlDocument();
                                    objXmlDictionaryReader.Read();
                                    objXmlDocument.LoadXml(objXmlDictionaryReader.ReadInnerXml());
                                }
                            }
                        }
                        //Console.WriteLine("HttpWebResponse.StatusDescription = {0}", objHttpWebResponse.StatusDescription);
                        break;
                    default:
                        Console.WriteLine("Default case, HttpWebResponse.StatusDescription = {0}", objHttpWebResponse.StatusDescription);
                        break;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (objHttpWebResponse != null)
                {
                    objHttpWebResponse.Close(); //this is critical because you can run out of connections if not closed. This is why we have timeout and it keeps working after process restart.
                }
            }

            return objXmlDocument;
        }
    }
}
