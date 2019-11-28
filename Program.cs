using System.Reflection;
using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.RegularExpressions;

namespace AWS.CodeDeploy.GlobalTool
{
    class Program
    {

        public static async Task<int> Main(params string[] args)
        {
            RootCommand rootCommand = new RootCommand(description: "Create AWS CodeDeploy EC2 deployments")
            {
                TreatUnmatchedTokensAsErrors = true
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
                Name = "app-path",
                Argument = new Argument<string>()
            };

            rootCommand.AddOption(regionOption);

            rootCommand.Handler = CommandHandler.Create<string, string, string, string, string>(Deploy);

            return await rootCommand.InvokeAsync(args);

        }

        static void Deploy(string application, string deploymentGroup, string s3Location, string appPath, string region)
        {

            Match match = Regex.Match(s3Location, "(s3://)(.*)/([a-zA-Z-.]*)$");

            if (!match.Success)
            {
                Console.WriteLine($"Invalid S3 Location: {s3Location}");
                return;
            }

            string bucketName = $"{match.Groups[2].Value}";
            string key = match.Groups[3].Value;

            string path = string.IsNullOrEmpty(appPath) ? "./" : appPath;
            var zipfile = ArchiveUtil.CreateZip(path);

            string eTag = S3Util.UploadRevision(s3Location, zipfile);
            string deploymentId = CodeDeployUtil.Deploy(application, deploymentGroup, bucketName, key, eTag.Replace("\"", ""));

            Console.WriteLine($"Created Deployment: {deploymentId}");
            // Optional: Redirect to AWS Console/Get Deployment Details
        }
    }
}
