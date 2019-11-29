using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Serilog;
using System.IO.Compression;

namespace AWS.CodeDeploy.Tool
{
    /// <summary>
    /// 
    /// </summary>
    public class ArchiveUtil
    {

        internal static FileInfo CreateZip(string localRevisionPath)
        {
            try
            {
                string outputPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
         Environment.GetEnvironmentVariable("temp") :
          "/tmp/";

                outputPath = $"{outputPath}/{Guid.NewGuid()}.zip";
                outputPath.Replace("\\", "/");
                localRevisionPath.Replace("\\", "/");

                //ZipFile.CreateFromDirectory(localRevisionPath, outputPath);
                ZipFile.CreateFromDirectory(localRevisionPath, outputPath, CompressionLevel.Fastest, false, new ZipEncoder());

                outputPath = outputPath.Replace("\\", "/");

                return new FileInfo(outputPath);
            }
            catch (System.Exception e)
            {
                Log.Error($"{e.GetBaseException().GetType().Name}: {e.Message}");
                return null;
            }

        }

    }

    internal class ZipEncoder : UTF8Encoding
    {
        public ZipEncoder() : base(true)
        {
        }

        public override byte[] GetBytes(string s)
        {
            s = s.Replace("\\", "/");
            return base.GetBytes(s);
        }
    }
}