using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;

namespace AWS.CodeDeploy.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class S3Util
    {

        static ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        static ILogger logger = loggerFactory.CreateLogger<Tool.S3Util>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s3Location"></param>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        public static string UploadRevision(string s3Location, FileStream zipStream)
        {
            try
            {
                Match match = Regex.Match(s3Location, "(s3://)(.*)/([a-zA-Z-.]*)$");

                string bucketName = $"{match.Groups[2].Value}";
                string key = match.Groups[3].Value;

                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = zipStream,
                    ContentType = "application/zip",
                    BucketName = bucketName,
                    Key = key
                };

                AmazonS3Client client = new AmazonS3Client();

                Task<PutObjectResponse> response = client.PutObjectAsync(request);
                Task.WaitAll(new Task[] { response });

                return response.Result.ETag;
            }
            catch (System.Exception e)
            {
                logger.LogError($"{e.Source}: {e.Message}");
                throw;
            }
        }
    }
}