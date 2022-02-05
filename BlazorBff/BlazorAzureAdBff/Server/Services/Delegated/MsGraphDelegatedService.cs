using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlazorAzureADWithApis.Server.Services.Delegated
{
    public class MsGraphDelegatedService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public MsGraphDelegatedService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User> GetGraphApiUser()
        {
            return await _graphServiceClient
                .Me
                .Request()
                .WithScopes("User.ReadBasic.All", "user.read")
                .GetAsync();
        }

        public async Task<string> GetGraphApiProfilePhoto()
        {
            try
            {
                var photo = string.Empty;
                // Get user photo
                using (var photoStream = await _graphServiceClient
                    .Me
                    .Photo
                    .Content
                    .Request()
                    .WithScopes("User.ReadBasic.All", "user.read")
                    .GetAsync())
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
    }
}

