using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AMS.REST.Test
{
    /*NOTES:
        Please see the doc for description.
        This sample solution demonstrates/contains 3 features:
             1.	Azure AD authentication via Client Credentials grant through only REST API;
             2.	Azure Media Services access through only REST API;
             3.	Azure Storage access through only REST API (as used for creating a media service account, again via REST API).
    
        ACS authentication via REST API code is commented but not deleted in case some customers still need that for some reason.
        AMS REST API Reference: http://msdn.microsoft.com/en-us/library/azure/hh973617.aspx 

        WHO:  willzhan
        WHOM: Those customers who do not use .NET/C# for developing apps accessing Azure Media Services
        WHEN: 6/2017
        WHAT: Sample code for AAD authentication, AMS access, Azure Storage access all via REST API without dependency on any special library.
        WHY:  ACS authN will be deprecated and customers are advised to migrate to AAD authentication for AMS access.
        DOC:  A companion doc has been created.
     */
    class Program
    {
        static void Main(string[] args)
        {
            //The commented code is for ACS authentication via REST API
            //string name = "willzhanmswest";
            //string name = "partnermedia1";
            //string key = System.Configuration.ConfigurationManager.AppSettings[name];
            //string mediaServicesApiServerUri = System.Configuration.ConfigurationManager.AppSettings["ApiServerUri"];         
            //string acsBearerToken = Utils.GetUrlEncodedAcsBearerToken(name, key);

            XmlDocument objXmlDocument;
            string FORMAT = "{0,-55}{1}";
            //string url, requestBody, path, assetName, assetId, resourcePath;
            //string processorId, preset, jobName, outputAssetName;
            //byte[] body;

            //AAD Authentication - Client Credentials grant
            string jwt = Utils.GetUrlEncodedJWT(System.Configuration.ConfigurationManager.AppSettings["clientid"], System.Configuration.ConfigurationManager.AppSettings["clientsecret"]);
            Console.WriteLine("JWT = {0}", jwt);
            string restapiuri = System.Configuration.ConfigurationManager.AppSettings["restapiuri"];
            //string authorizationcode = Utils.GetAuthorizationCode(System.Configuration.ConfigurationManager.AppSettings["clientid"], System.Configuration.ConfigurationManager.AppSettings["clientsecret"]);

            int id = 0; //test different AMS REST API with AAD authN
            switch (id)
            {
                case 0: //list MediaProcessors
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/MediaProcessors", null);
                    Console.WriteLine("{0,-55}{1,-55}{2,-55}{3,-55}", "ID", "Name", "Vendor", "Version");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine("{0,-55}{1,-55}{2,-55}{3,-55}", objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText, objXmlNode.ParentNode.SelectSingleNode("Vendor").InnerText, objXmlNode.ParentNode.SelectSingleNode("Version").InnerText);
                    }
                    break;
                case 1: //list ILocators
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/Locators", null);
                    Console.WriteLine(FORMAT, "ID", "PATH");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Path").InnerText);
                    }
                    break;
                //case 2: //list IPrograms
                //    objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "Programs", null);
                //    Console.WriteLine(FORMAT, "ID", "NAME");
                //    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                //    {
                //        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                //    }
                //    break;
                //case 3: //list IChannels
                //    objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "Channels", null);
                //    Console.WriteLine(FORMAT, "ID", "NAME");
                //    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                //    {
                //        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                //    }
                //    break;
                //case 4: //IChannel.Reset(): the channel must be started first
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Channels('nb:chid:UUID:d7835b7e-2848-4737-8fa5-e2b8608c518a')/Reset", null);   //willzhanmediaservice2
                //    break;
                //case 5: //update channel: the channel must be stopped first
                //    requestBody = string.Format("{{\"Output\":{{\"Hls\":{{\"FragmentsPerSegment\":{0}}}}}}}", "1");
                //    //requestBody = "{\"Output\":{\"Hls\":{\"FragmentsPerSegment\": 1}}}"; //the doc is incorrect
                //    objXmlDocument = AMSClient.MakeRestCall("PATCH", acsBearerToken, mediaServicesApiServerUri, "Channels('nb:chid:UUID:d7835b7e-2848-4737-8fa5-e2b8608c518a')", requestBody);   //willzhanmediaservice2
                //    break;
                //case 6: //update program
                //    requestBody = "{\"ArchiveWindowLength\":\"PT1H\"}";
                //    objXmlDocument = AMSClient.MakeRestCall("PATCH", acsBearerToken, mediaServicesApiServerUri, "Programs('nb:pgid:UUID:2a8bd3bc-ebd6-4f20-bb99-f924e1ca8995')", requestBody);   //willzhanmediaservice2
                //    break;
                ////STEPS to create an asset and upload a file
                //case 7: //create an empty IAsset
                //    requestBody = "{\"Name\":\"RESTTEST\"}";
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Assets", requestBody);   //willzhanmediaservice2
                //    break;
                //case 8: //set AccessPolicy for writing
                //    requestBody = "{\"Name\": \"RESTTESTAccessPolicy\", \"DurationInMinutes\" : \"3000\", \"Permissions\" : 2 }";
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "AccessPolicies", requestBody);
                //    break;
                case 9: //list assets
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/Assets", null);
                    Console.WriteLine(FORMAT, "ID", "NAME");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                    }
                    break;
                case 10: //list AccessPolicy
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/AccessPolicies", null);
                    Console.WriteLine(FORMAT, "ID", "NAME");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                    }
                    break;
                //case 11: //create SAS locator: Type: 1
                //    requestBody = "{\"AccessPolicyId\": \"nb:pid:UUID:fbc28148-3e93-4bc9-892e-03579aa6d1c6\", \"AssetId\" : \"nb:cid:UUID:d29545ff-0300-80bd-fc4c-f1e4b3b939f8\", \"StartTime\" : \"2015-02-12T16:45:53\", \"Type\" : 1 }";
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Locators", requestBody);
                //    break;
                case 12: //list locators
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/Locators", null);
                    Console.WriteLine("{0,-55}{1,-55}{2,-10}{3}", "LocatorId", "AssetId", "Type", "LocatorPath");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine("{0,-55}{1,-55}{2,-10}{3}", objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("AssetId").InnerText, objXmlNode.ParentNode.SelectSingleNode("Type").InnerText, objXmlNode.ParentNode.SelectSingleNode("Path").InnerText);
                    }
                    break;
                //case 13: //upload single file (Using Azure Storage REST API with SAS authentication)
                //    path = @"C:\Workspace\Destination\Input\SingileFile\RexonaCommercial.mp4";
                //    body = AzureStorageClient.GetBytesFromFile(path);
                //    url = "https://willzhanstorage2.blob.core.windows.net/asset-d29545ff-0300-80bd-fc4c-f1e4b3b939f8/resttest.mp4?sv=2012-02-12&sr=c&si=a5397716-ffce-4909-afc7-971cc01f940c&sig=su%2FrtoyG5Vm9oDOirx1I4GRHaLSmk25z1w0tjhAG17I%3D&st=2015-02-12T16%3A45%3A53Z&se=2015-02-14T18%3A45%3A53Z";
                //    objXmlDocument = AzureStorageClient.MakeRestCall("PUT", url, body);
                //    break;
                //case 14: //create IAsset metadata
                //    string query = string.Format("assetid='{0}'", System.Web.HttpUtility.UrlEncode("nb:cid:UUID:d29545ff-0300-80bd-fc4c-f1e4b3b939f8"));
                //    objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "CreateFileInfos", null, query: query);
                //    break;
                //case 15: //create a transcoding job
                //    processorId = "nb:mpid:UUID:1b1da727-93ae-4e46-a8a1-268828765609";  //from 0, use the latest version of Azure Media Encoder
                //    preset = "H264 Adaptive Bitrate MP4 Set 720p"; //https://msdn.microsoft.com/en-us/library/azure/dn619413.aspx
                //    jobName = "Zype_Job_01";
                //    outputAssetName = "Zype_Output_Asset_Name_01";
                //    requestBody = string.Format("{{\"Name\" : \"{3}\", \"InputMediaAssets\" : [{{\"__metadata\" : {{\"uri\" : \"https://media.windows.net/api/Assets('{0}')\"}}}}],  \"Tasks\" : [{{\"Configuration\" : \"{4}\", \"MediaProcessorId\" : \"{1}\",  \"TaskBody\" : \"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><taskBody><inputAsset>JobInputAsset(0)</inputAsset><outputAsset assetName=\\\"{2}\\\">JobOutputAsset(0)</outputAsset></taskBody>\"}}]}}", System.Web.HttpUtility.UrlEncode("nb:cid:UUID:e2ac45ff-0300-80bd-5ce6-f1e4b527b9fd"), processorId, outputAssetName, jobName, preset);
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Jobs", requestBody);
                //    break;
                //case 16: //create read access policy for origin locator, valid for about a year
                //    requestBody = "{\"Name\": \"ReadAccessPolicy\", \"DurationInMinutes\" : \"500000\", \"Permissions\" : 1 }";
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "AccessPolicies", requestBody);
                //    break;
                //case 17: //create origin locator: Type: 2
                //    requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"{2}\", \"Type\" : 2 }}", "nb:pid:UUID:8029a8c3-f845-4287-a998-befa81986453", "nb:cid:UUID:ae8245ff-0300-80bd-53b1-f1e4b5344be3", DateTime.UtcNow.AddDays(-1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Locators", requestBody);
                //    break;
                //case 18: //create a Indexer job
                //    processorId = "nb:mpid:UUID:233e57fc-36bb-4f6f-8f18-3b662747a9f8";  //Indexer processor ID, from case 0
                //    preset = System.IO.File.ReadAllText(@"..\..\MAVISConfig.xml");      //Indexer job needs a configuration file
                //    preset = preset.Replace("\"", "\\\"");                              //XML contains double quotes which need to be escaped as part of JSON values
                //    jobName = "MAVIS_Job_01";
                //    outputAssetName = "MAVIS_Output_Asset_01";
                //    assetId = "nb:cid:UUID:5310435d-1500-80c3-53b9-f1e4be2cf85d";       //asset ID of uploaded MP4 (LyncSkypeSizzleVideo750k-mp4-Source)
                //    requestBody = string.Format("{{\"Name\" : \"{3}\", \"InputMediaAssets\" : [{{\"__metadata\" : {{\"uri\" : \"https://media.windows.net/api/Assets('{0}')\"}}}}],  \"Tasks\" : [{{\"Configuration\" : \"{4}\", \"MediaProcessorId\" : \"{1}\",  \"TaskBody\" : \"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><taskBody><inputAsset>JobInputAsset(0)</inputAsset><outputAsset assetName=\\\"{2}\\\">JobOutputAsset(0)</outputAsset></taskBody>\"}}]}}", System.Web.HttpUtility.UrlEncode(assetId), processorId, outputAssetName, jobName, preset);
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Jobs", requestBody);
                //    break;
                //case 101:
                //    path = @"C:\Workspace\Destination\Input\SingileFile\RexonaCommercial.mp4";
                //    assetName = "ZypeRest07";
                //    RunWorkflow(assetName, path, acsBearerToken, mediaServicesApiServerUri);
                //    break;
                case 20:  //list StreamingEndpoint
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/StreamingEndpoints", null);
                    Console.WriteLine("{0,-55}{1,-55}{2,-20}{3}", "Id", "Name", "ScaleUnits", "HostName");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine("{0,-55}{1,-55}{2,-20}{3}", objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText, objXmlNode.ParentNode.SelectSingleNode("ScaleUnits").InnerText, objXmlNode.ParentNode.SelectSingleNode("HostName").InnerText);
                    }
                    break;
                //case 21: //scale StreamingEndpoint
                //    requestBody = "{\"scaleUnits\" : 1}";  //“scaleUnit” in this case is not a property on an Entity, but rather a parameter passed to the Scale action on an StreamingEndpoint entity (The POST request below is to invoke the action). I believe the parameter name has to internally exactly match (unfortunately not case insensitive match) the parameter name in the signature of our C# method that gets called for this action. As coding convention, first letter of a parameter should be lower case, which led to this effect.
                //    resourcePath = string.Format("StreamingEndpoints('{0}')/Scale", System.Web.HttpUtility.UrlEncode("nb:oid:UUID:364e57ac-35be-0d26-05f5-6fd00b3f33fd"));
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, resourcePath, requestBody);
                //    break;
                //case 22: //create a channel
                //    requestBody = "{\"Id\":null,\"Name\":\"testchannel001\",\"Description\":\"\",\"Created\":\"2015-01-01T00:00:00\",\"LastModified\":\"2015-01-01T00:00:00\",\"State\":null,\"Input\":{\"KeyFrameInterval\":null,\"StreamingProtocol\":\"FragmentedMP4\",\"AccessControl\":{\"IP\":{\"Allow\":[{\"Name\":\"testName1\",\"Address\":\"0.0.0.0\",\"SubnetPrefixLength\":0}]}},\"Endpoints\":[]},\"Preview\":{\"AccessControl\":{\"IP\":{\"Allow\":[{\"Name\":\"testName1\",\"Address\":\"0.0.0.0\",\"SubnetPrefixLength\":0}]}},\"Endpoints\":[]},\"Output\":{\"Hls\":{\"FragmentsPerSegment\":1}},\"CrossSiteAccessPolicies\":{\"ClientAccessPolicy\":null,\"CrossDomainPolicy\":null}}";
                //    resourcePath = "Channels";
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, resourcePath, requestBody);
                //    break;
                //case 30: //create Azure storage account
                //    CreateAzureStorageAccount();
                //    break;
                //case 31: //create media service account
                //    CreateMediaServiceAccount();
                //    break;
                //case 32: //GetAccountDetails
                //    ManagementRESTAPIHelper helper = new ManagementRESTAPIHelper("https://management.core.windows.net", "A3022B01D70924D819CBD0DC118D22ACAC29DE36", "2db39a66-f5c5-4b03-9d1c-71018352379b");
                //    helper.GetAccountDetails("willzhanmswest");
                //    break;
                //case 33: //AAD AuthN - Service Principal
                //    jwt = Utils.GetUrlEncodedJWT(System.Configuration.ConfigurationManager.AppSettings["clientid"], System.Configuration.ConfigurationManager.AppSettings["clientsecret"]);
                //    Console.WriteLine(jwt);
                //    break;
                default:
                    break;
            }

            Console.WriteLine("Hit any key to finish");
            Console.ReadKey();
        }

        private static void RunWorkflow(string assetName, string path, string acsBearerToken, string mediaServicesApiServerUri)
        {
            //variables
            string requestBody, assetId = string.Empty, policyId = string.Empty, policyName, locatorPath = string.Empty;
            XmlDocument objXmlDocument;

            //create empty IAsset
            requestBody = string.Format("{{\"Name\":\"{0}\"}}", assetName);
            objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Assets", requestBody);
            Console.WriteLine("An empty IAsset with name {0} is created.", assetName);

            //get assetId
            assetId = GetAssetId(assetName, acsBearerToken, mediaServicesApiServerUri);
            Console.WriteLine("IAsset.Id = {0}", assetId);

            //create write access policy for SAS
            policyName = string.Format("{0}_sas_access_policy", assetName);
            requestBody = string.Format("{{\"Name\": \"{0}\", \"DurationInMinutes\" : \"3000\", \"Permissions\" : 2 }}", policyName);
            objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "AccessPolicies", requestBody);
            Console.WriteLine("Access policy with name {0} is created.", policyName);

            //get policyId
            policyId = GetAccessPolicyId(policyName, acsBearerToken, mediaServicesApiServerUri);
            Console.WriteLine("For SAS URI: IAccessPolicy.Name = {0}, IAccessPolicy.Id = {1}", policyName, policyId);

            //create SAS locator
            requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"{2}\", \"Type\" : 1 }}", policyId, assetId, DateTime.UtcNow.AddDays(-1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
            objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Locators", requestBody);
            Console.WriteLine("SAS locator created with policy Id = {0} for asset Id = {1}", policyId, assetId);

            //get SAS locator path
            objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "Locators", null);
            foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
            {
                if ((objXmlNode.ParentNode.SelectSingleNode("AssetId").InnerText == assetId) && (objXmlNode.ParentNode.SelectSingleNode("Type").InnerText == "1"))   //SAS, Type=1
                {
                    locatorPath = objXmlNode.ParentNode.SelectSingleNode("Path").InnerText;
                    break;
                }
            }
            Console.WriteLine("SAS locator path = {0}", locatorPath);

            //modify locator path to point to the mezzanine
            string filename = System.IO.Path.GetFileName(path);
            locatorPath = locatorPath.Replace("?", "/" + filename + "?");
            Console.WriteLine("Full SAS URI for upload mezzanine = {0}", locatorPath);

            //upload single file (Using Azure Storage REST API with SAS authentication)
            byte[] body = AzureStorageClient.GetBytesFromFile(path);
            Console.WriteLine("File upload starts ...");
            objXmlDocument = AzureStorageClient.MakeRestCall("PUT", locatorPath, body);
            Console.WriteLine("File upload completes");

            //create IAsset metadata
            string query = string.Format("assetid='{0}'", System.Web.HttpUtility.UrlEncode(assetId));
            objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "CreateFileInfos", null, query: query);
            Console.WriteLine("Mezzanine asset creation completes, ready to be transcoded.");

            //create and a transcoding job
            string processorId = "nb:mpid:UUID:1b1da727-93ae-4e46-a8a1-268828765609";  //from case 0, use the latest version of Azure Media Encoder
            string preset = "H264 Adaptive Bitrate MP4 Set 720p";                      //from https://msdn.microsoft.com/en-us/library/azure/dn619413.aspx
            string jobName = "Zype_Job_01";
            string outputAssetName = string.Format("Output_Asset_{0}", assetName);
            //double quotes in XML need to be escaped in the output
            requestBody = string.Format("{{\"Name\" : \"{3}\", \"InputMediaAssets\" : [{{\"__metadata\" : {{\"uri\" : \"https://media.windows.net/api/Assets('{0}')\"}}}}],  \"Tasks\" : [{{\"Configuration\" : \"{4}\", \"MediaProcessorId\" : \"{1}\",  \"TaskBody\" : \"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><taskBody><inputAsset>JobInputAsset(0)</inputAsset><outputAsset assetName=\\\"{2}\\\">JobOutputAsset(0)</outputAsset></taskBody>\"}}]}}", System.Web.HttpUtility.UrlEncode(assetId), processorId, outputAssetName, jobName, preset);
            objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Jobs", requestBody);
            Console.WriteLine("AMS encoder job submitted with preset: \"{0}\"", preset);

            //create read access policy for origin locator, valid for about a year
            policyName = string.Format("{0}_origin_access_policy", assetName);
            requestBody = string.Format("{{\"Name\": \"{0}\", \"DurationInMinutes\" : \"500000\", \"Permissions\" : 1 }}", policyName);
            objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "AccessPolicies", requestBody);

            //retrieve the read access policy ID
            policyId = GetAccessPolicyId(policyName, acsBearerToken, mediaServicesApiServerUri);
            Console.WriteLine("For origin URI: IAccessPolicy.Name = {0}, IAccessPolicy.Id = {1}", policyName, policyId);

            //get job output asset ID to be published
            assetId = GetAssetId(outputAssetName, acsBearerToken, mediaServicesApiServerUri);

            //create origin locator: Type: 2
            requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"{2}\", \"Type\" : 2 }}", policyId, assetId, DateTime.UtcNow.AddDays(-1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
            objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, "Locators", requestBody);
            Console.WriteLine("Asset {0} has been published", assetId);

            //get origin locator path
            objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "Locators", null);
            foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
            {
                if ((objXmlNode.ParentNode.SelectSingleNode("AssetId").InnerText == assetId) && (objXmlNode.ParentNode.SelectSingleNode("Type").InnerText == "2"))   //origin, Type=2
                {
                    locatorPath = objXmlNode.ParentNode.SelectSingleNode("Path").InnerText;
                    break;
                }
            }
            Console.WriteLine("Origin locator path = {0}", locatorPath);
            Console.WriteLine("Origin URL: {0}{1}.ism/manifest", locatorPath, System.IO.Path.GetFileNameWithoutExtension(path));
            Console.WriteLine("Playback URL: {0}?url={1}{2}.ism/manifest", "http://openidconnectweb.azurewebsites.net/OpenAMPlayer", locatorPath, System.IO.Path.GetFileNameWithoutExtension(path));
        }

        private static string GetAccessPolicyId(string policyName, string acsBearerToken, string mediaServicesApiServerUri)
        {
            string policyId = string.Empty;
            XmlDocument objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "AccessPolicies", null);
            foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
            {
                if (objXmlNode.ParentNode.SelectSingleNode("Name").InnerText.ToLower() == policyName.ToLower())
                {
                    policyId = objXmlNode.InnerText;
                    break;
                }
            }

            return policyId;
        }

        private static string GetAssetId(string assetName, string acsBearerToken, string mediaServicesApiServerUri)
        {
            string assetId = string.Empty;
            XmlDocument objXmlDocument = AMSClient.MakeRestCall("GET", acsBearerToken, mediaServicesApiServerUri, "Assets", null);
            foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
            {
                if (objXmlNode.ParentNode.SelectSingleNode("Name").InnerText.ToLower() == assetName.ToLower())
                {
                    assetId = objXmlNode.InnerText;
                    break;
                }
            }
            return assetId;
        }


        //If you get HTTP409: media service name has a conflict with an existing one.
        private static void CreateMediaServiceAccount()
        {
            ManagementRESTAPIHelper helper = new ManagementRESTAPIHelper("https://management.core.windows.net",
                                                                         "A3022B01D70924D819CBD0DC118D22ACAC29DE36",
                                                                         "2db39a66-f5c5-4b03-9d1c-71018352379b");

            // Initialize the AccountInfo class.
            MediaServicesAccount accountInfo = new MediaServicesAccount();
            accountInfo.AccountName = "testwzms";
            accountInfo.Region = "West US";
            accountInfo.StorageAccountName = "testwzstor";
            accountInfo.StorageAccountKey = "aPlz4nSy3utNCESR9rTjN2E1QUJnpFPXnE2iRhx8D05HXpQ25X5bNcPMUL/8/k+owfOtTpMCC9gjkXYIImmV8A==";
            accountInfo.BlobStorageEndpointUri = "https://testwzstor.blob.core.windows.net/";


            AttachStorageAccountRequest storageAccountToAttach = new AttachStorageAccountRequest();
            storageAccountToAttach.StorageAccountName = "testwzstor";
            storageAccountToAttach.StorageAccountKey = "aPlz4nSy3utNCESR9rTjN2E1QUJnpFPXnE2iRhx8D05HXpQ25X5bNcPMUL/8/k+owfOtTpMCC9gjkXYIImmV8A==";
            storageAccountToAttach.BlobStorageEndpointUri = "https://testwzstor.blob.core.windows.net/";

            // Call CreateMediaServiceAccountUsingXmlContentType to create a new Media Services account. 
            helper.CreateMediaServiceAccountUsingXmlContentType(accountInfo);

            // Call AttachStorageAccountToMediaServiceAccount to attach an existing storage account to the Media Services account.
            //helper.AttachStorageAccountToMediaServiceAccount(accountInfo, storageAccountToAttach);

            // Call the following methods to get details about Media Services account.
            //helper.GetAccountDetails(accountInfo);
            //helper.ListAvailableRegions(accountInfo);
            //helper.ListSubscriptionAccounts(accountInfo);
        }

        /*In progress for 0 seconds
            Succeeded: Operation 09a42978d21aa0dc999d0b6aa9c0b67a completed after 20 seconds with status 200 (OK)
            New Storage Account Properties for testwzstor:
            <StorageService xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
              <Url>https://management.core.windows.net/2db39a66-f5c5-4b03-9d1c-71018352379b/services/storageservices/testwzstor</Url>
              <ServiceName>testwzstor</ServiceName>
              <StorageServiceProperties>
                <Description>testwzstor desc</Description>
                <Location>West US</Location>
                <Label>testwzstor label</Label>
                <Status>Created</Status>
                <Endpoints>
                  <Endpoint>http://testwzstor.blob.core.windows.net/</Endpoint>
                  <Endpoint>http://testwzstor.queue.core.windows.net/</Endpoint>
                  <Endpoint>http://testwzstor.table.core.windows.net/</Endpoint>
                </Endpoints>
              </StorageServiceProperties>
            </StorageService>
         */
        private static void CreateAzureStorageAccount()
        {
            string storageName = "testwzstor";
            string storageDesc = "testwzstor desc";
            string storageLabel = "testwzstor label";
            string region = "West US";
            string thumbprint = "A3022B01D70924D819CBD0DC118D22ACAC29DE36";
            AzureStorageRESTAPIHelper.Certificate = AzureStorageRESTAPIHelper.GetStoreCertificate(thumbprint);
            string requestId = AzureStorageRESTAPIHelper.CreateStorageAccount(storageName, storageDesc, storageLabel, null, region, false);
            // Loop on Get Operation Status for result of storage creation
            AzureStorageRESTAPIHelper.OperationResult result = AzureStorageRESTAPIHelper.PollGetOperationStatus(
                requestId,
                pollIntervalSeconds: 20,
                timeoutSeconds: 180);
            switch (result.Status)
            {
                case OperationStatus.TimedOut:
                    Console.WriteLine(
                        "Poll of Get Operation Status timed out: " +
                        "Operation {0} is still in progress after {1} seconds.",
                        requestId,
                        (int)result.RunningTime.TotalSeconds);
                    break;

                case OperationStatus.Failed:
                    Console.WriteLine(
                        "Failed: Operation {0} failed after " +
                        "{1} seconds with status {2} ({3}) - {4}: {5}",
                        requestId,
                        (int)result.RunningTime.TotalSeconds,
                        (int)result.StatusCode,
                        result.StatusCode,
                        result.Code,
                        result.Message);
                    break;

                case OperationStatus.Succeeded:
                    Console.WriteLine(
                        "Succeeded: Operation {0} completed " +
                        "after {1} seconds with status {2} ({3})",
                        requestId,
                        (int)result.RunningTime.TotalSeconds,
                        (int)result.StatusCode,
                        result.StatusCode);
                    break;
            }

            // Display the property values for the new storage account. Convert the Label property to a readable value for display.
            XElement updatedProperties = AzureStorageRESTAPIHelper.GetStorageAccountProperties(storageName);
            XNamespace wa = "http://schemas.microsoft.com/windowsazure";
            XElement labelElement = updatedProperties.Descendants(wa + "Label").First();
            labelElement.Value = labelElement.Value.FromBase64();
            Console.WriteLine(
                "New Storage Account Properties for {0}:{1}{2}",
                storageName,
                Environment.NewLine,
                updatedProperties.ToString(SaveOptions.OmitDuplicateNamespaces));
        }
    } //class
}  //namespace
