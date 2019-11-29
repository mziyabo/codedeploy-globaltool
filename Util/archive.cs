using Serilog;
using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Runtime.InteropServices;

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
               
                ZipFile.CreateFromDirectory(localRevisionPath, outputPath);

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