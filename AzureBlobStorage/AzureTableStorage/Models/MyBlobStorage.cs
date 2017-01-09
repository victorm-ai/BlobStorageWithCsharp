using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;

namespace AzureTableStorage.Models
{
	public class MyBlobStorage
	{
        private string Account { get; set; }
        private string Key { get; set; }

        public MyBlobStorage()
        {
            this.Account = "testaccounttoday";
            this.Key = "Z/ddM9f7mIl1bm9yVRnzmENUPbW6yg8o0zEMgRtK4Mn1iSau5y4xuwK7pe2z/nzzxhwBFt4OTBLeSBu7+hycAA==";
        }

        public void Create_container()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
        }

        public void Upload_a_blob_into_a_container()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // Create or overwrite the "myblob" blob with contents from a local file.

            string fullPath= Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "pin.png";
            using (var fileStream = System.IO.File.OpenRead(fullPath))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        public void List_the_blobs_in_a_container()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }
        }

        public void Download_blobs_way1()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");       

            // Retrieve reference to a blob named "photo1.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(@"C:\FolderCreated\pin2.png"))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public void Download_blobs_way2_string()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Retrieve reference to a blob named "myblob.txt"
            CloudBlockBlob blockBlob2 = container.GetBlockBlobReference("myblob");

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob2.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public void Delete_blobs()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // Delete the blob.
            blockBlob.Delete();
        }

        public async void List_blobs_in_pages_asynchronously()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();


            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            //List blobs to the console window, with paging.
            Console.WriteLine("List blobs in pages:");

            int i = 0;
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 10, continuationToken, null, null);
                if (resultSegment.Results.Count<IListBlobItem>() > 0) { Console.WriteLine("Page {0}:", ++i); }
                foreach (var blobItem in resultSegment.Results)
                {
                    Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
                }
                Console.WriteLine();

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        public void Writing_to_an_append_blob()
        {
            //Parse the connection string for the storage account.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            //Create service client for credentialed access to the Blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("my-append-blobs");

            //Create the container if it does not already exist.
            container.CreateIfNotExists();

            //Get a reference to an append blob.
            CloudAppendBlob appendBlob = container.GetAppendBlobReference("append-blob.log");

            //Create the append blob. Note that if the blob already exists, the CreateOrReplace() method will overwrite it.
            //You can check whether the blob exists to avoid overwriting it by using CloudAppendBlob.Exists().
            appendBlob.CreateOrReplace();

            int numBlocks = 10;

            //Generate an array of random bytes.
            Random rnd = new Random();
            byte[] bytes = new byte[numBlocks];
            rnd.NextBytes(bytes);

            //Simulate a logging operation by writing text data and byte data to the end of the append blob.
            for (int i = 0; i < numBlocks; i++)
            {
                appendBlob.AppendText(String.Format("Timestamp: {0:u} \tLog Entry: {1}{2}",
                    DateTime.UtcNow, bytes[i], Environment.NewLine));
            }

            //Read the append blob to the console window.
            Console.WriteLine(appendBlob.DownloadText());
        }

    }
}