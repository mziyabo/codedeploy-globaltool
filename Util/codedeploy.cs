using System.Threading.Tasks;
using Amazon.CodeDeploy;
using Amazon.CodeDeploy.Model;
using S3Location = Amazon.CodeDeploy.Model.S3Location;

namespace AWS.CodeDeploy.Tool
{

    /// <summary>
    /// 
    /// </summary>
    public class CodeDeployUtil
    {
        GetDeploymentResponse GetDeployment(string deploymentId)
        {
            GetDeploymentRequest request = new GetDeploymentRequest()
            {
                DeploymentId = deploymentId
            };

            AmazonCodeDeployClient client = new AmazonCodeDeployClient();

            Task<GetDeploymentResponse> response = client.GetDeploymentAsync(request);
            Task.WaitAll(new Task[] { response });

            return response.Result;
        }

        internal static string Deploy(string applicationName, string deploymentGroupName, string bucket, string key, string eTag)
        {
            S3Location location = new S3Location()
            {
                Bucket = bucket,
                BundleType = BundleType.Zip,
                ETag = eTag,
                Key = key
            };

            CreateDeploymentRequest request = new CreateDeploymentRequest()
            {
                ApplicationName = applicationName,
                DeploymentGroupName = deploymentGroupName,
                Revision = new RevisionLocation()
                {
                    S3Location = location,
                    RevisionType = RevisionLocationType.S3
                },

            };

            AmazonCodeDeployClient client = new AmazonCodeDeployClient();

            Task<CreateDeploymentResponse> response = client.CreateDeploymentAsync(request);
            Task.WaitAll(new Task[] { response });

            return response.Result.DeploymentId;
            
        }
    }
}