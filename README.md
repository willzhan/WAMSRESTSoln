### Azure AD Authentication for Azure Media Services Access: Both via REST API
For some customers, we need a way to use only REST API for Azure AD authentication and subsequent Azure Media Services access, without relying on any of Azure AD libraries or Azure Media Services .NET SDK. This sample presents such a solution. Since the code is all REST API calls without dependency on any Azure AD or Azure Media Services library, the code can be easily translated to any other programming languages.

The sample code has 2 parts:
1.	A DLL library project containing all the REST API code for Azure AD authentication. It also contains a method for making REST API calls to Azure Media Services REST API endpoint, with access_token.
2.	A console test client which initiates Azure AD authentication and calls 11 different AMS REST API.

The sample project demonstrates/contains 3 features:
1.	Azure AD authentication via Client Credentials grant through only REST API;
2.	Azure Media Services access through only REST API;
3.	Azure Storage access through only REST API (as used for creating a media service account, again via REST API).

For setup and other info, please see the companion document published in Azure Document Center - Azure Media Services section: [Use AAD authentication to access Azure Media Services API with REST](https://docs.microsoft.com/en-us/azure/media-services/media-services-rest-connect-with-aad?branch=pr-en-us-15547), under Use AAD Auth to Access API/Access API with REST.
