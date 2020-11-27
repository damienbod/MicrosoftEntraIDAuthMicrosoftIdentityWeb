using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiWithMutlipleApis.Services
{
    public class GraphApiClientDirect
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IHttpClientFactory _clientFactory;

        public GraphApiClientDirect(ITokenAcquisition tokenAcquisition,
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<User> GetGraphApiUser()
        {
            var graphclient = await GetGraphClient(new string[] { "User.ReadBasic.All", "user.read" })
               .ConfigureAwait(false);

            return await graphclient.Me.Request().GetAsync().ConfigureAwait(false);
        }

        public async Task<string> GetGraphApiProfilePhoto()
        {
            try
            {
                var graphclient = await GetGraphClient(new string[] { "User.ReadBasic.All", "user.read" })
               .ConfigureAwait(false);

                var photo = string.Empty;
                // Get user photo
                using (var photoStream = await graphclient.Me.Photo
                    .Content.Request().GetAsync().ConfigureAwait(false))
                {
                    byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                    photo = Convert.ToBase64String(photoByte);
                }

                return photo;
            }
            catch
            {
                return string.Empty;
            }   
        }

        public async Task<string> GetSharepointFile()
        {
            var graphclient = await GetGraphClient(
                new string[] { "user.read", "AllSites.Read" }
            ).ConfigureAwait(false);

            var user = await graphclient.Me.Request().GetAsync().ConfigureAwait(false);

            if (user == null)
                throw new ArgumentException($"User not found in AD.");

            var sharepointDomain = "damienbodtestsharing.sharepoint.com";
            var relativePath = "/sites/TestDoc";
            var fileName = "aad_ms_login_02.png";

            var site = await graphclient
                .Sites[sharepointDomain]
                .SiteWithPath(relativePath)
                .Request()
                .GetAsync().ConfigureAwait(false);

            var drive = await graphclient
                .Sites[site.Id]
                .Drive
                .Request()
                .GetAsync().ConfigureAwait(false);

            var items = await graphclient
                .Sites[site.Id]
                .Drives[drive.Id]
                .Root
                .Children
                .Request().GetAsync().ConfigureAwait(false);

            var file = items
                .FirstOrDefault(f => f.File != null && f.WebUrl.Contains(fileName));

            var stream = await graphclient
                .Sites[site.Id]
                .Drives[drive.Id]
                .Items[file.Id].Content
                .Request()
                .GetAsync().ConfigureAwait(false);

            var fileAsString = StreamToString(stream);
            return fileAsString;
        }

        private async Task<GraphServiceClient> GetGraphClient(string[] scopes)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(
             scopes).ConfigureAwait(false);

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://graph.microsoft.com/beta");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GraphServiceClient graphClient = new GraphServiceClient(client)
            {
                AuthenticationProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                })
            };

            graphClient.BaseUrl = "https://graph.microsoft.com/beta";
            return graphClient;
        }

        private static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
