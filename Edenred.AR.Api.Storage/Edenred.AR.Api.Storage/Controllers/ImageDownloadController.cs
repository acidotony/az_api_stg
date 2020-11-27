using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;

namespace Edenred.AR.Api.Storage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageDownloadController : ControllerBase
    {
        public static IWebHostEnvironment _enviroment;
        public ImageDownloadController(IWebHostEnvironment enviroment)
        {
            _enviroment = enviroment;
        }

        public class FileDownloadApi
        {
            public IFormFile files { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=earst01dev;AccountKey=2aEcxobBkJ8rXgcOZR9QjKvh2pgxtUwbM7vEwJZu8O8yhZlnjLznmeoVLI0KhOURVqJ12XLdql7hKdQHkLlHZw==;EndpointSuffix=core.windows.net");
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference(@"web-comercios");

            // Ensure that the share exists.
            if (share.Exists() && !String.IsNullOrEmpty(fileName) && fileName.Length > 4)
            {
                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference(@"facturas");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference(fileName);

                    // Ensure that the file exists.
                    if (await file.ExistsAsync())
                    {
                        byte[] myDataBuffer = new byte[file.Properties.Length];
                        file.DownloadToByteArray(myDataBuffer, 0);
                        return File(myDataBuffer, GetContentType(fileName), true);
                    }
                    else { return BadRequest(false); }
                }
                else { return BadRequest(false); }
            }
            return BadRequest(false);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}