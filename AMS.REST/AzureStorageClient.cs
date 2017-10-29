using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;
using System.Runtime.Serialization.Json;

namespace AMS.REST
{
    public class AzureStorageClient
    {
        public static XmlDocument UploadBlobWithRestAPISasPermissionOnBlobContainer(string blobContainerSasUri, string blobName, byte[] blobContent)
        {
            XmlDocument objXmlDocument = null;
            int contentLength = blobContent.Length;
            string queryString = (new Uri(blobContainerSasUri)).Query;
            string blobContainerUri = blobContainerSasUri.Split('?')[0];
            string requestUri = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}{2}", blobContainerUri, blobName, queryString);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = "PUT";
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            request.ContentLength = contentLength;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(blobContent, 0, contentLength);
            }
            using (HttpWebResponse objHttpWebResponse = (HttpWebResponse)request.GetResponse())
            {
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
            }

            return objXmlDocument;
        }


        public static XmlDocument MakeRestCall(string verb, string url, byte[] body)
        {
            XmlDocument objXmlDocument = null;
            HttpWebResponse objHttpWebResponse = null;
            try
            {
                Console.WriteLine("Making storage REST call to {0}", url);
                var request = AzureStorageClient.GenerateRequest(verb, url, body);
                objHttpWebResponse = (HttpWebResponse)request.GetResponse();

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
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                objHttpWebResponse.Close(); //this is critical because you can run out of connections if not closed. This is why we have timeout and it keeps working after process restart.
            }

            return objXmlDocument;
        }

        public static HttpWebRequest GenerateRequest(string verb,
                                                     string url,
                                                     byte[] body)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.AllowAutoRedirect = false; //We manage our own redirects.
            request.Method = verb;

            request.ContentType = "application/atom+xml";  // "application/json;odata=verbose";
            //request.Headers.Add("x-ms-date", DateTime.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-blob-type", "BlockBlob");

            if (body != null)
            {
                request.ContentLength = body.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();
            }
            else
            {
                request.ContentLength = 0;
            }
            return request;
        }

        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            FileStream objFileStream = File.OpenRead(fullFilePath);
            try
            {
                byte[] bytes = new byte[objFileStream.Length];
                objFileStream.Read(bytes, 0, Convert.ToInt32(objFileStream.Length));
                objFileStream.Close();
                return bytes;
            }
            finally
            {
                objFileStream.Close();
            }

        }
    }
}
