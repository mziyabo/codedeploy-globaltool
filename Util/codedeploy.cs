using System;
using Amazon.CodeDeploy;
using Amazon.CodeDeploy.Model;
using Amazon.S3.Model;

namespace AWS.CodeDeploy.GlobalTool
{

    /// <summary>
    /// 
    /// </summary>
    public class CodeDeployUtil
    {
        GetDeploymentResponse GetDeployment(string deploymentId)
        {
            throw new NotImplementedException();
        }

        internal static CreateDeploymentResponse Deploy(string applicationName, string deploymentGroupName, PutObjectResponse revisionLocation)
        {
            throw new NotImplementedException();
        }

    }
}