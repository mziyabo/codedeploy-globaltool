using System.Reflection;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.RegularExpressions;
using System.IO;
using Serilog;
using System.IO.Compression;

namespace AWS.CodeDeploy.Tool
{
    class Program
    {


        public static async Task<int> Main(params string[] args)
        {

            Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .CreateLogger();

            RootCommand rootCommand = new RootCommand(description: "Create AWS CodeDeploy Deployments")
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

            // Deploy(args[0], args[1], args[2], "", "");
            // return 1;
        }

        static void Deploy(string application, string deploymentGroup, string s3Location, string appPath, string region)
        {

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
                string eTag = S3Util.UploadRevision(s3Location, zipFileStream);

                //string eTag = S3Util.UploadRevision(s3Location, zipFile.FullName);

                if (!string.IsNullOrEmpty(eTag))
                {
                    string deploymentId = CodeDeployUtil.Deploy(application, deploymentGroup, bucketName, key, eTag.Replace("\"", ""));

                    // Cleanup
                    zipFileStream.Close();
                    zipFile.Delete();
                }
            }

            // Optional: Redirect to AWS Console/Get Deployment Details
        }
    }
}
