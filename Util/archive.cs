using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace AWS.CodeDeploy.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class ArchiveUtil
    {
        internal static FileStream CreateZip(string localRevisionPath)
        {

            string outputPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
             Environment.GetEnvironmentVariable("temp") :
              "/tmp/";
            
            outputPath = $"{outputPath}/{Guid.NewGuid()}.zip";
            
            ZipFile.CreateFromDirectory(localRevisionPath, outputPath);

            return File.Open(outputPath, FileMode.OpenOrCreate);
        }
    }
}