using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Configuration;

namespace Edenred.AR.Api.Storage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        public static IWebHostEnvironment _enviroment;
        public IConfiguration Configuration { get; }
        public ImageUploadController(IWebHostEnvironment enviroment, IConfiguration configuration)
        {
            _enviroment = enviroment;
            Configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] string input)
        {
            string sRequest = String.Empty;
            using (StreamReader stmRequest = new StreamReader(Request.Body, Encoding.UTF8))
            {
                sRequest = await stmRequest.ReadToEndAsync();
            }

            Dictionary<string, object> dict = JsonSerializer.Deserialize<Dictionary<string, object>>(sRequest);
            string name = dict["FILE_NAME"].ToString();
            string shareReference = dict["SHARE_REFERENCE"].ToString();
            string directoryReference = dict["DIRECTORY_REFERENCE"].ToString();
            string image = dict["IMAGE_DATA"].ToString();
            byte[] buffer = Convert.FromBase64String(image);

            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration.GetSection("AzureAppConfiguration").GetSection("ConnectionString").Value);
                CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

                // Get a reference to the file share we created previously.
                CloudFileShare share = fileClient.GetShareReference(shareReference);

                // Ensure that the share exists.
                if (share.Exists())
                {
                    // Get a reference to the root directory for the share.
                    CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                    // Get a reference to the directory we created previously.
                    CloudFileDirectory cloudSubDirectory = rootDir.GetDirectoryReference(directoryReference);

                    // Ensure that the directory exists.
                    if (cloudSubDirectory.Exists())
                    {
                        //Create a reference to the filename that you will be uploading
                        CloudFile cloudFile = cloudSubDirectory.GetFileReference(name);

                        //Upload the file to Azure.
                        await cloudFile.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
                        return Ok(true);
                    }
                    else
                    {
                        return BadRequest(true);
                    }
                }
                return BadRequest(true);

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
