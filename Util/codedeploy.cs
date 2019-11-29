using System.Threading.Tasks;
using Amazon;
using Amazon.CodeDeploy;
using Amazon.CodeDeploy.Model;
using Serilog;
using S3Location = Amazon.CodeDeploy.Model.S3Location;

namespace AWS.CodeDeploy.Tool
{

    /// <summary>
    /// 
    /// </summary>
    public class CodeDeployUtil
    {
        internal static GetDeploymentResponse GetDeployment(string deploymentId, string region)
        {
            try
            {
                GetDeploymentRequest request = new GetDeploymentRequest()
                {
                    DeploymentId = deploymentId
                };

                AmazonCodeDeployClient client = string.IsNullOrEmpty(region) ?
                 new AmazonCodeDeployClient() :
                 new AmazonCodeDeployClient(RegionEndpoint.GetBySystemName(region));

                Task<GetDeploymentResponse> response = client.GetDeploymentAsync(request);
                Task.WaitAll(new Task[] { response });

                return response.Result;
            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
                return null;
            }
        }

        internal static string Deploy(string applicationName, string deploymentGroupName, string bucket, string key, string eTag, string version, string region = "")
        {
            try
            {
                S3Location location = new S3Location()
                {
                    Bucket = bucket,
                    BundleType = BundleType.Zip,
                    ETag = eTag,
                    Key = key,
                    Version = version
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

                AmazonCodeDeployClient client = string.IsNullOrEmpty(region) ?
                 new AmazonCodeDeployClient() :
                 new AmazonCodeDeployClient(RegionEndpoint.GetBySystemName(region));

                Task<CreateDeploymentResponse> response = client.CreateDeploymentAsync(request);
                Task.WaitAll(new Task[] { response });


                string message = "Deployment Created: {0} \ndotnet codedeploy status --deployment-id {0}";
                if (!string.IsNullOrEmpty(region))
                {
                    message += " --region {1}";
                }

                Log.Information(message, response.Result.DeploymentId, region);
                return response.Result.DeploymentId;

            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
                return null;
            }
        }
    }
}