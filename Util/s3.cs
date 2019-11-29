using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Serilog;

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
        /// <param name="s3Location"></param>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        public static string UploadRevision(string s3Location, FileStream zipStream)
        {
            try
            {
                Match match = Regex.Match(s3Location, "(s3://)(.*)/([a-zA-Z-1-9\\.]*)$");

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

                Log.Information("Uploaded Revision to {0} etag: {1} version: {2}",s3Location,response.Result.ETag,response.Result.VersionId);
                return response.Result.ETag;
            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
                return null;
            }
        }
    


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s3Location"></param>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        public static string UploadRevision(string s3Location, string path)
        {
            try
            {
                Match match = Regex.Match(s3Location, "(s3://)(.*)/([a-zA-Z-1-9\\.]*)$");

                string bucketName = $"{match.Groups[2].Value}";
                string key = match.Groups[3].Value;

                PutObjectRequest request = new PutObjectRequest()
                {
                    FilePath = path,
                    ContentType = "application/zip",
                    BucketName = bucketName,
                    Key = key
                };

                AmazonS3Client client = new AmazonS3Client();

                Task<PutObjectResponse> response = client.PutObjectAsync(request);
                Task.WaitAll(new Task[] { response });

                Log.Information("Uploaded Revision to {0} etag: {1} version: {2}",s3Location,response.Result.ETag,response.Result.VersionId);
                return response.Result.ETag;
            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
                return null;
            }
        }


        
    }
}