using System.Reflection;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.RegularExpressions;
using System.IO;
using Serilog;
using Amazon.S3.Model;
using Amazon.CodeDeploy.Model;
using System.Linq;

namespace AWS.CodeDeploy.Tool
{
    class Program
    {


        public static async Task<int> Main(params string[] args)
        {

            Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .CreateLogger();

            RootCommand rootCommand = new RootCommand(description: "Create AWS CodeDeploy deployments")
            {
                TreatUnmatchedTokensAsErrors = false
            };

            Option appOption = new Option(
               aliases: new string[] { "--application" }
              , description: "AWS CodeDeploy application name")
            {
                Argument = new Argument<string>(),
                Required = true
            };

            rootCommand.AddOption(appOption);

            Option deployGroupOption = new Option(
              aliases: new string[] { "--deployment-group" }
              , description: "Deployment Group Name")
            {
                Argument = new Argument<string>(),
                Required = true
            };

            rootCommand.AddOption(deployGroupOption);

            Option bucketOption = new Option(
             aliases: new string[] { "--s3-location" }
             , description: "S3 Revision Location s3://<bucket>/<key>")
            {
                Argument = new Argument<string>(),
                Required = true
            };

            rootCommand.AddOption(bucketOption);

            Option localPathOption = new Option(
             aliases: new string[] { "--app-path" }
             , description: "Local application path to deploy. Default './'.")
            {
                Name = "app-path",
                Argument = new Argument<string>()
            };
            rootCommand.AddOption(localPathOption);

            Option regionOption = new Option(
            aliases: new string[] { "--region" }
            , description: "AWS Region e.g. us-east-1")
            {
                Argument = new Argument<string>()
            };

            rootCommand.AddOption(regionOption);

            Command statusCommand = new Command("status", description: "Get Deployment Status")
            {
                TreatUnmatchedTokensAsErrors = true
            };

            Option deploymentOption = new Option(
            aliases: new string[] { "--deployment-id" }
            , description: "Deployment Id")
            {
                Argument = new Argument<string>(),
                Required = true
            };

            statusCommand.AddOption(deploymentOption);
            statusCommand.AddOption(regionOption);
            statusCommand.Handler = CommandHandler.Create<string, string>(GetDeploymentStatus);

            rootCommand.AddCommand(statusCommand);

            rootCommand.Handler = CommandHandler.Create<string, string, string, string, string>(Deploy);
            return await rootCommand.InvokeAsync(args);

        }

        static void Deploy(string application, string deploymentGroup, string s3Location, string appPath, string region)
        {
            try
            {
                if (!IsDotnetDirectory())
                {
                    Log.Warning("No .NET project found in directory {0}", Directory.GetCurrentDirectory());
                    return;
                }

                Match match = Regex.Match(s3Location, "(s3://)(.*)/([a-zA-Z-1-9\\.]*)$");

                if (!match.Success)
                {
                    Log.Error("Invalid S3 Location: {0}. Expected s3://<bucket-name>/<key>", s3Location);
                    return;
                }

                string bucketName = $"{match.Groups[2].Value}";
                string key = match.Groups[3].Value;

                string path = string.IsNullOrEmpty(appPath) ? "./" : appPath;

                Log.Information("Zipping app-path {0}", path);
                FileInfo zipFile = ArchiveUtil.CreateZip(path);

                FileStream zipFileStream = zipFile.Open(FileMode.Open);

                if (zipFileStream != null)
                {
                    PutObjectResponse response = S3Util.UploadRevision(bucketName, key, zipFileStream);

                    if (response != null)
                    {
                        string deploymentId = CodeDeployUtil.Deploy(application, deploymentGroup, bucketName, key, response.ETag.Replace("\"", ""), response.VersionId, region);

                        zipFileStream.Close();
                        zipFile.Delete();
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
            }
            // Optional: Redirect to AWS Console/Get Deployment Details
        }

        static void GetDeploymentStatus(string deploymentId, string region = "")
        {
            GetDeploymentResponse response = CodeDeployUtil.GetDeployment(deploymentId, region);
            if (response != null)
            {
                Log.Information("{0} {1} - CompleteTime: {2}", deploymentId, response.DeploymentInfo.Status, response.DeploymentInfo.CompleteTime);
            }
        }


        static bool IsDotnetDirectory()
        {
            string directory = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(directory);

            return files.Any(file => Regex.Match(file, ".*.(cs|vb)proj").Success);
        }
    }
}
