using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TMS.BusinessLayer.DTO;

namespace TMS.BusinessLayer.Infra
{
    public class MinioService : IMinioService
    {
        private IMinioClient _minioClient;
        private IHttpContextAccessor _httpContextAccessor;
        private string _bucketName;
       

        public MinioService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            var minioInfo = _httpContextAccessor.HttpContext.Items["MinioInfo"] as MinioConnectionInfo;

            if (minioInfo == null)
            {
                throw new Exception("Minio connection info is not resolved");
            }

            _bucketName = minioInfo.BucketName;

            _minioClient = new MinioClient()
                                    .WithEndpoint(minioInfo.EndPoint)
                                    .WithCredentials(minioInfo.AccessKey, minioInfo.SecretKey)
                                    .Build();
        }


        public async Task<byte[]> DownloadObjectAsync(string objectName)
        {
            try
            {
                var res = new byte[] { };
                // Confirm object exists before attempting to get
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName);
                var stat = await _minioClient.StatObjectAsync(statObjectArgs);


                // Create a MemoryStream to store the object data
                using (var memoryStream = new MemoryStream())
                {
                    // Get input stream to have content of the object
                    GetObjectArgs getObjectArgs = new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream) =>
                    {
                        // Copy the object data to the memory stream
                        await stream.CopyToAsync(memoryStream);
                    });
                    var objData = await _minioClient.GetObjectAsync(getObjectArgs);

                    // Convert the memory stream to a byte array
                    res = memoryStream.ToArray();
                }

                return res;
            } catch
            {
                throw new Exception("Error while downloading file from minio");
            }
        }
    }
}
