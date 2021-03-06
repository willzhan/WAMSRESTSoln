﻿using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AMS.REST.Test
{
    /*NOTES:
        Please see my Azure doc for details: https://docs.microsoft.com/en-us/azure/media-services/media-services-rest-connect-with-aad
        This sample solution demonstrates/contains 3 features:
             1.	Azure AD authentication using Service Principal via Client Credentials grant through only REST API;
             2.	Azure Media Services access through only REST API;
             3.	Azure Storage access through only REST API (as used for creating a media service account, again via REST API).
    
        ACS authentication via REST API code is commented but not deleted in case some customers still need that for some reason.
        AMS REST API Reference: http://msdn.microsoft.com/en-us/library/azure/hh973617.aspx 

        WHO:  willzhan
        WHOM: Those customers who do not use .NET/C# for developing apps accessing Azure Media Services
        WHEN: 6/2017
        WHAT: Sample code for AAD authentication, AMS access, Azure Storage access all via REST API without dependency on any special library.
        WHY:  ACS authN will be deprecated and customers are advised to migrate to AAD authentication for AMS access.
        DOC:  A companion doc has been created in Azure Doc Center. https://docs.microsoft.com/en-us/azure/media-services/media-services-rest-connect-with-aad
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
            string requestBody, path;
            string assetId, policyId;
            //string url, requestBody, path, assetName, assetId, resourcePath;
            //string processorId, preset, jobName, outputAssetName;
            //byte[] body;

            //AAD Authentication - Client Credentials grant
            string jwt = Utils.GetUrlEncodedJWT(System.Configuration.ConfigurationManager.AppSettings["clientid"], System.Configuration.ConfigurationManager.AppSettings["clientsecret"]);
            //Console.WriteLine("JWT = {0}", jwt);
            string restapiuri = System.Configuration.ConfigurationManager.AppSettings["restapiuri"];
            //string authorizationcode = Utils.GetAuthorizationCode(System.Configuration.ConfigurationManager.AppSettings["clientid"], System.Configuration.ConfigurationManager.AppSettings["clientsecret"]);

            int id = 101; //choose an integer between 0 and 10 to test 11 AMS REST API calls with JWT token
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
                case 2:  //list StreamingEndpoints
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/StreamingEndpoints", null);
                    Console.WriteLine("{0,-55}{1,-55}{2,-20}{3}", "Id", "Name", "ScaleUnits", "HostName");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine("{0,-55}{1,-55}{2,-20}{3}", objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText, objXmlNode.ParentNode.SelectSingleNode("ScaleUnits").InnerText, objXmlNode.ParentNode.SelectSingleNode("HostName").InnerText);
                    }
                    break;
                case 3: //list IAssets
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/Assets", null);
                    Console.WriteLine(FORMAT, "ID", "NAME");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                    }
                    break;
                case 4: //list AccessPolicy
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/AccessPolicies", null);
                    Console.WriteLine(FORMAT, "ID", "NAME");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                    }
                    break;
                case 5: //list IChannels
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/Channels", null);
                    Console.WriteLine(FORMAT, "ID", "NAME");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                    }
                    break;
                case 6: //list IPrograms
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "/Programs", null);
                    Console.WriteLine(FORMAT, "ID", "NAME");
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
                    {
                        Console.WriteLine(FORMAT, objXmlNode.InnerText, objXmlNode.ParentNode.SelectSingleNode("Name").InnerText);
                    }
                    break;
                case 7: //IChannel.Reset(): the channel must be started first
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "/Channels('nb:chid:UUID:5c077dd2-e7f2-4814-8f53-e432c1d9f06a')/Reset", null);
                    break;               
                case 8: //update program
                    requestBody = "{\"ArchiveWindowLength\":\"PT1H\"}";
                    objXmlDocument = AMSClient.MakeRestCall("PATCH", jwt, restapiuri, "/Programs('nb:pgid:UUID:a3f5e002-3cb9-4d7e-acd3-349d64c4c863')", requestBody);
                    break;
                case 9: //update channel: the channel must be stopped first
                    requestBody = string.Format("{{\"Output\":{{\"Hls\":{{\"FragmentsPerSegment\":{0}}}}}}}", "1");
                    //requestBody = "{\"Output\":{\"Hls\":{\"FragmentsPerSegment\": 1}}}"; //the doc is incorrect
                    objXmlDocument = AMSClient.MakeRestCall("PATCH", jwt, restapiuri, "/Channels('nb:chid:UUID:5c077dd2-e7f2-4814-8f53-e432c1d9f06a')", requestBody);
                    break;
                //STEPS to create an asset and upload a file
                case 10: //create an empty IAsset
                    requestBody = "{\"Name\":\"REST_test_asset\"}";
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "/Assets", requestBody);  
                    break;
                case 11: //set AccessPolicy for writing
                    requestBody = "{\"Name\": \"RESTTestAccessPolicy\", \"DurationInMinutes\" : \"3000\", \"Permissions\" : 2 }";
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "AccessPolicies", requestBody);
                    Console.WriteLine(objXmlDocument.OuterXml);
                    break;
                case 12: //create SAS locator: Type: 1
                    assetId  = "nb:cid:UUID:5da1d9dc-b157-46d8-9d59-37139f152972";
                    policyId = "nb:pid:UUID:cde5c349-c2e6-4de9-84b2-3a6548fac1a7";
                    requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"2017-10-24T16:45:53\", \"Type\" : 1 }}", policyId, assetId);
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Locators", requestBody);
                    Console.WriteLine(objXmlDocument.OuterXml);
                    break;
                case 13: //upload single file (Using Azure Storage REST API with SAS authentication)
                    path = @"C:\Workspace\Destination\Input\SingileFile\RexonaCommercial.mp4";  
                    byte[] body = AzureStorageClient.GetBytesFromFile(path);
                    //SAS URI of the empty container
                    string sasUri = "https://partnerstorage1.blob.core.windows.net/asset-5da1d9dc-b157-46d8-9d59-37139f152972?sv=2012-02-12&sr=c&si=ad67bb92-d163-43ce-b60c-d1b150342265&sig=XHatAd16dI6apWDM%2B6SiAq0S1uo7stF5xwhMVDUSjvk%3D&st=2017-10-24T16%3A45%3A53Z&se=2017-10-26T18%3A45%3A53Z";
                    //objXmlDocument = AzureStorageClient.MakeRestCall("PUT", url, body);
                    objXmlDocument = AzureStorageClient.UploadBlobWithRestAPISasPermissionOnBlobContainer(sasUri, "RexonaCommercial.mp4", body);
                    break;
                case 14: //create IAsset metadata
                    assetId = "nb:cid:UUID:5da1d9dc-b157-46d8-9d59-37139f152972";
                    string query = string.Format("assetid='{0}'", System.Web.HttpUtility.UrlEncode(assetId));
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "CreateFileInfos", null, query: query);
                    break;
                case 15: //create a transcoding job
                    string processorId = "nb:mpid:UUID:ff4df607-d419-42f0-bc17-a481b1331e56";  //from id=0, use the latest version of Azure Media Encoder
                    string preset = "Content Adaptive Multiple Bitrate MP4"; //https://docs.microsoft.com/en-us/azure/media-services/media-services-mes-presets-overview
                    string jobName = "RETS_test_job01";
                    string outputAssetName = "REST_test_output";
                    assetId = "nb:cid:UUID:5da1d9dc-b157-46d8-9d59-37139f152972";
                    requestBody = string.Format("{{\"Name\" : \"{3}\", \"InputMediaAssets\" : [{{\"__metadata\" : {{\"uri\" : \"https://media.windows.net/api/Assets('{0}')\"}}}}],  \"Tasks\" : [{{\"Configuration\" : \"{4}\", \"MediaProcessorId\" : \"{1}\",  \"TaskBody\" : \"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><taskBody><inputAsset>JobInputAsset(0)</inputAsset><outputAsset assetName=\\\"{2}\\\">JobOutputAsset(0)</outputAsset></taskBody>\"}}]}}", System.Web.HttpUtility.UrlEncode(assetId), processorId, outputAssetName, jobName, preset);
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Jobs", requestBody);
                    break;
                case 16: //create read access policy for origin locator, valid for about a year
                    requestBody = "{\"Name\": \"ReadAccessPolicy\", \"DurationInMinutes\" : \"500000\", \"Permissions\" : 1 }";
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "AccessPolicies", requestBody);
                    Console.WriteLine(objXmlDocument.OuterXml);
                    break;
                case 17: //create origin locator: Type: 2
                    policyId = "nb:pid:UUID:db236678-29de-4a33-9e9f-9d10440a1722";
                    assetId = "nb:cid:UUID:4f99a238-5da0-4e5a-b0f8-e60dc13c6406";
                    requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"{2}\", \"Type\" : 2 }}", policyId, assetId, DateTime.UtcNow.AddDays(-1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
                    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Locators", requestBody);
                    Console.WriteLine(objXmlDocument.OuterXml);
                    break;
                case 18: //list asset files (to get .ism file to build streaming URL)
                    assetId = "nb:cid:UUID:4f99a238-5da0-4e5a-b0f8-e60dc13c6406";
                    objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, string.Format("Assets('{0}')/Files", assetId), null);
                    Console.WriteLine(objXmlDocument.OuterXml);
                    foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Name"))
                    {
                        Console.WriteLine("{0,-55}", objXmlNode.InnerText);
                    }
                    break;
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
                case 101:
                    path = @"C:\Workspace\Destination\Input\SingileFile\RexonaCommercial.mp4";
                    string assetName = "RexonaCommercialRest";
                    RunWorkflow(assetName, path, jwt, restapiuri);
                    break;
                //case 21: //scale StreamingEndpoint
                //    requestBody = "{\"scaleUnits\" : 1}";  //“scaleUnit” in this case is not a property on an Entity, but rather a parameter passed to the Scale action on an StreamingEndpoint entity (The POST request below is to invoke the action). I believe the parameter name has to internally exactly match (unfortunately not case insensitive match) the parameter name in the signature of our C# method that gets called for this action. As coding convention, first letter of a parameter should be lower case, which led to this effect.
                //    resourcePath = string.Format("StreamingEndpoints('{0}')/Scale", System.Web.HttpUtility.UrlEncode("nb:oid:UUID:364e57ac-35be-0d26-05f5-6fd00b3f33fd"));
                //    objXmlDocument = AMSClient.MakeRestCall("POST", acsBearerToken, mediaServicesApiServerUri, resourcePath, requestBody);
                //    break;
                //case 10: //create a channel
                //    requestBody = "{\"Id\":null,\"Name\":\"testchannel002\",\"Description\":\"\",\"Created\":\"2017-06-01T00:00:00\",\"LastModified\":\"2017-06-14T00:00:00\",\"State\":null,\"Input\":{\"KeyFrameInterval\":null,\"StreamingProtocol\":\"FragmentedMP4\",\"AccessControl\":{\"IP\":{\"Allow\":[{\"Name\":\"testName1\",\"Address\":\"0.0.0.0\",\"SubnetPrefixLength\":0}]}},\"Endpoints\":[]},\"Preview\":{\"AccessControl\":{\"IP\":{\"Allow\":[{\"Name\":\"testName1\",\"Address\":\"0.0.0.0\",\"SubnetPrefixLength\":0}]}},\"Endpoints\":[]},\"Output\":{\"Hls\":{\"FragmentsPerSegment\":1}},\"CrossSiteAccessPolicies\":{\"ClientAccessPolicy\":null,\"CrossDomainPolicy\":null}}";
                //    objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "/Channels", requestBody);
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
                default:
                    break;
            }

            Console.WriteLine("Hit any key to finish");
            Console.ReadKey();
        }

        private static void RunWorkflow(string assetName, string path, string jwt, string restapiuri)
        {
            //variables
            string requestBody, assetId = string.Empty, policyId = string.Empty, policyName, locatorPath = string.Empty;
            XmlDocument objXmlDocument;

            //create empty IAsset
            requestBody = string.Format("{{\"Name\":\"{0}\"}}", assetName);
            objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Assets", requestBody);
            Console.WriteLine("An empty IAsset with name {0} is created.", assetName);

            //get assetId
            assetId = GetAssetId(assetName, jwt, restapiuri);
            Console.WriteLine("IAsset.Id = {0}", assetId);

            //create write access policy for SAS
            policyName = string.Format("{0}_sas_access_policy", assetName);
            requestBody = string.Format("{{\"Name\": \"{0}\", \"DurationInMinutes\" : \"3000\", \"Permissions\" : 2 }}", policyName);
            objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "AccessPolicies", requestBody);
            Console.WriteLine("Access policy with name {0} is created.", policyName);

            //get policyId
            policyId = GetAccessPolicyId(policyName, jwt, restapiuri);
            Console.WriteLine("For SAS URI: IAccessPolicy.Name = {0}, IAccessPolicy.Id = {1}", policyName, policyId);

            //create SAS locator
            requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"{2}\", \"Type\" : 1 }}", policyId, assetId, DateTime.UtcNow.AddDays(-1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
            objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Locators", requestBody);
            Console.WriteLine("SAS locator created with policy Id = {0} for asset Id = {1}", policyId, assetId);

            //get SAS locator path
            objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "Locators", null);
            foreach (XmlNode objXmlNode in objXmlDocument.GetElementsByTagName("Id"))
            {
                if ((objXmlNode.ParentNode.SelectSingleNode("AssetId").InnerText == assetId) && (objXmlNode.ParentNode.SelectSingleNode("Type").InnerText == "1"))   //SAS, Type=1
                {
                    locatorPath = objXmlNode.ParentNode.SelectSingleNode("Path").InnerText;
                    break;
                }
            }
            Console.WriteLine("SAS locator path = {0}", locatorPath);

            //modify SAS locator path to point to the mezzanine in Azure Storage
            string filename = System.IO.Path.GetFileName(path);
            //locatorPath = locatorPath.Replace("?", "/" + filename + "?");  //this is not needed, this is done inside AzureStorageClient.UploadBlobWithRestAPISasPermissionOnBlobContainer
            Console.WriteLine("Full SAS URI for upload mezzanine = {0}", locatorPath);

            //upload single file (Using Azure Storage REST API with SAS authentication)
            byte[] body = AzureStorageClient.GetBytesFromFile(path);
            Console.WriteLine("File upload starts ...");
            //objXmlDocument = AzureStorageClient.MakeRestCall("PUT", locatorPath, body);
            objXmlDocument = AzureStorageClient.UploadBlobWithRestAPISasPermissionOnBlobContainer(locatorPath, filename, body);
            Console.WriteLine("File upload completes");

            //create IAsset metadata
            string query = string.Format("assetid='{0}'", System.Web.HttpUtility.UrlEncode(assetId));
            objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "CreateFileInfos", null, query: query);
            Console.WriteLine("Mezzanine asset creation completes, ready to be transcoded.");

            //create and a transcoding job
            string processorId = "nb:mpid:UUID:ff4df607-d419-42f0-bc17-a481b1331e56";  //from id=0, use the latest version of Azure Media Encoder
            string preset = "Content Adaptive Multiple Bitrate MP4"; //https://docs.microsoft.com/en-us/azure/media-services/media-services-mes-presets-overview
            string jobName = string.Format("{0}_encode_job", assetName);
            string outputAssetName = string.Format("Output_Asset_{0}", assetName);
            requestBody = string.Format("{{\"Name\" : \"{3}\", \"InputMediaAssets\" : [{{\"__metadata\" : {{\"uri\" : \"https://media.windows.net/api/Assets('{0}')\"}}}}],  \"Tasks\" : [{{\"Configuration\" : \"{4}\", \"MediaProcessorId\" : \"{1}\",  \"TaskBody\" : \"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?><taskBody><inputAsset>JobInputAsset(0)</inputAsset><outputAsset assetName=\\\"{2}\\\">JobOutputAsset(0)</outputAsset></taskBody>\"}}]}}", System.Web.HttpUtility.UrlEncode(assetId), processorId, outputAssetName, jobName, preset);
            objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Jobs", requestBody);
            Console.WriteLine("AMS encoder job submitted with preset: \"{0}\"", preset);

            //create read access policy for origin locator, valid for about a year
            policyName = string.Format("{0}_origin_access_policy", assetName);
            requestBody = string.Format("{{\"Name\": \"{0}\", \"DurationInMinutes\" : \"500000\", \"Permissions\" : 1 }}", policyName);
            objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "AccessPolicies", requestBody);

            //retrieve the read access policy ID
            policyId = GetAccessPolicyId(policyName, jwt, restapiuri);
            Console.WriteLine("For origin URI: IAccessPolicy.Name = {0}, IAccessPolicy.Id = {1}", policyName, policyId);

            //get job output asset ID to be published
            assetId = GetAssetId(outputAssetName, jwt, restapiuri);

            //create origin locator: Type: 2
            requestBody = string.Format("{{\"AccessPolicyId\": \"{0}\", \"AssetId\" : \"{1}\", \"StartTime\" : \"{2}\", \"Type\" : 2 }}", policyId, assetId, DateTime.UtcNow.AddDays(-1.0).ToString("yyyy-MM-ddTHH:mm:ss"));
            objXmlDocument = AMSClient.MakeRestCall("POST", jwt, restapiuri, "Locators", requestBody);
            Console.WriteLine("Asset {0} has been published", assetId);

            //get origin locator path
            objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "Locators", null);
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
            Console.WriteLine("Playback URL: {0}?url={1}{2}.ism/manifest", "http://aka.ms/amtest", locatorPath, System.IO.Path.GetFileNameWithoutExtension(path));
        }

        private static string GetAccessPolicyId(string policyName, string jwt, string restapiuri)
        {
            string policyId = string.Empty;
            XmlDocument objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "AccessPolicies", null);
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

        private static string GetAssetId(string assetName, string jwt, string restapiuri)
        {
            string assetId = string.Empty;
            XmlDocument objXmlDocument = AMSClient.MakeRestCall("GET", jwt, restapiuri, "Assets", null);
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
