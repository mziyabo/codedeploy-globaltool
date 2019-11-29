using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace AWS.CodeDeploy.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class ArchiveUtil
    {
        string path;

        static ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        static ILogger logger = loggerFactory.CreateLogger<Tool.S3Util>();

        internal static FileStream CreateZip(string localRevisionPath)
        {

            string outputPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
         Environment.GetEnvironmentVariable("temp") :
          "/tmp/";

            outputPath = $"{outputPath}/{Guid.NewGuid()}.zip";

            ZipFile.CreateFromDirectory(localRevisionPath, outputPath);

            FileStream zipStream = File.Open(outputPath, FileMode.Open);
            // zipStream.Close();
            // File.Delete(outputPath);

            return zipStream;

        }
    }
}