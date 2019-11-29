using Amazon.S3;
using Amazon.S3.Model;
using Serilog;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AWS.CodeDeploy.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class S3Util
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key"></param>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        public static PutObjectResponse UploadRevision(string bucketName, string key, FileStream zipStream)
        {
            try
            {
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

                string eTag = response.Result.ETag.Replace("\"", "");


                string message = string.IsNullOrEmpty(response.Result.VersionId) ?
                $"s3://{bucketName}/{key}?eTag={eTag}" :
                $"s3://{bucketName}/{key}?versionId={response.Result.VersionId.Replace("\"", "")}&eTag={eTag}";

                Log.Information("Uploaded Revision: {0}", message);
                return response.Result;
            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
                return null;
            }
        }
    }
}