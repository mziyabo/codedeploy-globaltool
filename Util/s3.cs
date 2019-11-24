using System;
using Amazon.S3;
using Amazon.S3.Model;

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
        /// <param name="zipfile"></param>
        /// <returns></returns>
        public static PutObjectResponse UploadRevision(string s3Location, object zipfile)
        {
            throw new NotImplementedException();
        }
    }
}