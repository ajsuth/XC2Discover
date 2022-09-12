using Ajsuth.Sample.Discover.Engine.Policies;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Service
{
    public class CloudStorageService
    {
        private bool isInitialized = false;
        private string baseUrl;

        public CloudStorageService(CloudStoragePolicy cloudStoragePolicy)
        {
            baseUrl = cloudStoragePolicy.BaseUrl;
            CloudStorageAccount.TryParse(cloudStoragePolicy.ConnectionString, out var storage);
            Client = storage.CreateCloudBlobClient();
            Container = Client.GetContainerReference(cloudStoragePolicy.Container);
        }

        public CloudBlobClient Client { get; }

        public CloudBlobContainer Container { get; }

        public async Task Save(string reference, byte[] bytes, string fileType = null, string cacheControl = null)
        {
            await this.Init();
            var block = Container.GetBlockBlobReference(reference);
            if (fileType != null)
            {
                block.Properties.ContentType = fileType;
            }

            await block.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }

        private async Task Init()
        {
            if (isInitialized)
            {
                return;
            }

            var created = await Container.CreateIfNotExistsAsync();
            if (created)
            {
                var permissions = await Container.GetPermissionsAsync();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                await Container.SetPermissionsAsync(permissions);

                var properties = await Client.GetServicePropertiesAsync();
                properties.Cors.CorsRules.Add(new CorsRule
                {
                    AllowedHeaders = { "*" },
                    AllowedOrigins = { "*" },
                    AllowedMethods =
                        CorsHttpMethods.Options |
                        CorsHttpMethods.Get |
                        CorsHttpMethods.Put |
                        CorsHttpMethods.Post |
                        CorsHttpMethods.Head |
                        CorsHttpMethods.Delete |
                        CorsHttpMethods.Merge,
                    ExposedHeaders = { "*" },
                    MaxAgeInSeconds = (int)TimeSpan.FromHours(1).TotalSeconds,
                });
                await Client.SetServicePropertiesAsync(properties);
            }

            isInitialized = true;
        }

        public string GetBaseUrl()
        {
            return baseUrl;
        }
    }
}
