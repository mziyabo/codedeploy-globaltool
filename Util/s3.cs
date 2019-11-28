using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

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
        public static async Task<string> UploadRevision(string s3Location, FileStream zipStream)
        {
            Match match = Regex.Match(s3Location, "(s3://)(.*)/([a-zA-Z-.]*)$");

            RegionEndpoint region = null;
            BucketRegionDetector.BucketRegionCache.TryGetValue("singwm-temp-delete", out region);

            AmazonS3Client client = new AmazonS3Client();

            string bucketName = $"{match.Groups[2].Value}";
            string key = match.Groups[3].Value;

            PutObjectRequest request = new PutObjectRequest()
            {
                InputStream = zipStream,
                ContentType = "application/zip",
                BucketName = bucketName,
                Key = key
            };

            Task<PutObjectResponse> response = client.PutObjectAsync(request);
            Task.WaitAll(new Task[] { response });
            
            return response.Result.ETag;
        }
    }
}