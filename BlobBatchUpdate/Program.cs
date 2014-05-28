using CommandLine;
using Microsoft.Win32;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlobBatchUpdate
{
    class Program
    {
        static Dictionary<string, string> _cacheContentTypes = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            Options options = new Options();
            Parser parser = new Parser();
            if (parser.ParseArgumentsStrict(args, options, () => ShowUsage(options)))
            {
                if (string.IsNullOrEmpty(options.StorageName) || string.IsNullOrEmpty(options.StorageKey))
                {
                    ShowUsage(options);
                }
                Run(options);
            }
        }

        private static void ShowUsage(Options options)
        {
            Console.WriteLine(options.GetUsage());
            Environment.Exit(1);
        }

        private static void Run(Options options)
        {
            string customBlobEndpoint = string.Format("BlobEndpoint=http://{0}.blob.core.chinacloudapi.cn/;", options.StorageName);
            string customEndpoint = options.UseChinaEndpoint ? customBlobEndpoint : "";
            string connectionString = string.Format("{0}DefaultEndpointsProtocol=http;AccountName={1};AccountKey={2}",
                                                    customEndpoint, options.StorageName, options.StorageKey);
            CloudStorageAccount storageAccount;
            CloudStorageAccount.TryParse(connectionString, out storageAccount);
            if (storageAccount == null)
            {
                Console.WriteLine("Storage account is invalid");
                Environment.Exit(1);
            }
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            foreach (CloudBlobContainer container in blobClient.ListContainers())
            {
                if (container.Name == "wad-control-container") continue;
                BatchUpdateBobProperties(container);
            }
        }

        private static void BatchUpdateBobProperties(CloudBlobContainer container)
        {
            Console.WriteLine("Found container {0}, starting ...", container.Name);
            int count = 0;
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item is CloudBlockBlob)
                {
                    CloudBlockBlob blob = item as CloudBlockBlob;
                    if (TryUpdateBlobProperty(blob))
                    {
                        count++;
                    }
                }
            }
            if (count > 0)
            {
                Console.WriteLine("Successfully updated {0} blobs in container {1}", count, container.Name);
            }
            else if (count == 0)
            {
                Console.WriteLine("All blobs in container {0} has corect content types", container.Name);
            }

        }

        private static bool TryUpdateBlobProperty(CloudBlockBlob blob)
        {
            string extension = Path.GetExtension(blob.Uri.AbsoluteUri);
            if (extension != null)
            {
                if (!_cacheContentTypes.ContainsKey(extension))
                {
                    _cacheContentTypes[extension] = GetContentType(extension);
                }

                if (_cacheContentTypes[extension] != null)
                {
                    string newContentType = _cacheContentTypes[extension];
                    if (blob.Properties.ContentType != newContentType)
                    {
                        Console.WriteLine("Updating content type to {0}, url = {1}", newContentType, blob.Uri);
                        blob.Properties.ContentType = newContentType;
                        blob.SetProperties();
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetContentType(string exteision)
        {
            string keyname = string.Format(@"HKEY_CLASSES_ROOT\{0}", exteision);
            string type = Registry.GetValue(keyname, "Content Type", null) as string;
            return type;
        }
    }
}
