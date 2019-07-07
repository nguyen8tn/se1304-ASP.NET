using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PRN292Prj.Data
{
    public class AzureCloud
    {
        public IConfiguration Configuration { get; private set; }

        public AzureCloud(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        public string UploadFile(string fileName, Stream stream) 
        {
            string uri;
            try
            {
                string conn = Configuration.GetValue<string>("AzureStorage:ConnectionString");
                string containerName = Configuration.GetValue<string>("AzureStorage:Container");
                CloudStorageAccount account = CloudStorageAccount.Parse(conn);
                CloudBlobClient client = account.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.UploadFromStreamAsync(stream);
                uri = blockBlob.Uri.ToString();
            }
            catch (Exception)
            {
                throw;
            } 
            return uri;

        }
        public string getSAS()
        {
            string SAS;
            try
            {
                string conn = Configuration.GetValue<string>("AzureStorage:ConnectionString");
                string containerName = Configuration.GetValue<string>("AzureStorage:Container");
                CloudStorageAccount account = CloudStorageAccount.Parse(conn);
                CloudBlobClient client = account.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(containerName);
                SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(45),
                };

                SAS = container.GetSharedAccessSignature(policy);
            }
            catch (Exception)
            {
                throw;
            }
            return SAS;
        }
    }
}
