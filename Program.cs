using System;
using Amazon.S3.Model;
using Amazon.CodeDeploy.Model;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace AWS.CodeDeploy.Tool
{
    class Program
    {
        string localRevisionPath;
        string s3RevisionLocation;
        private string applicationName;
        private string deploymentGroupName;

        public static async Task<int> Main(params string[] args)
        {
            RootCommand rootCommand = new RootCommand(description: "Deploy application through AWS CodeDeploy")
            {
                TreatUnmatchedTokensAsErrors = true
            };

            Option appOption = new Option(
              aliases: new string[] { "--application" }
              , description: "AWS CodeDeploy Application name")
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
             , description: "S3 revision location s3://<bucket>/<key>")
            {
                Argument = new Argument<string>(),
                Required = true
            };

            rootCommand.AddOption(bucketOption);

            Option localPathOption = new Option(
             aliases: new string[] { "--app-path" }
             , description: "Local application path to deploy. Defaul './'.")
            {
                Argument = new Argument<FileInfo>()
            };

            rootCommand.AddOption(localPathOption);

            rootCommand.Handler =
              CommandHandler.Create<string, string, string, FileInfo>(Deploy);

            return await rootCommand.InvokeAsync(args);
        }

        static void Deploy(string applicationName, string deploymentGroupName, string s3Location, FileInfo localRevisionPath)
        {
            // TODO: Implement

            //var zipfile = ArchiveUtil.CreateZip(localRevisionPath.FullName);
            //PutObjectResponse s3Revision = S3Util.UploadRevision(s3Location, zipfile);
            //CreateDeploymentResponse deploymentResponse = CodeDeployUtil.Deploy(applicationName, deploymentGroupName, s3Revision);

            // Optional: Redirect to AWS Console/Get Deployment Details
        }
    }
}
