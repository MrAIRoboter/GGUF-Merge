using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGUFMerge.Utils.Extensions
{
    public static class FileInfoExtensions
    {
        public static void AppendFile(this FileInfo source, FileInfo appendFileInfo)
        {
            if (source.Exists == false)
                new FileNotFoundException("Source file was not found!", source.FullName);

            if (appendFileInfo.Exists == false)
                new FileNotFoundException("Append file was not found!", appendFileInfo.FullName);

            using (FileStream sourceFileStream = new FileStream(source.FullName, FileMode.Append, FileAccess.Write))
            using (FileStream targetFileStream = new FileStream(appendFileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                targetFileStream.CopyTo(sourceFileStream);
            }
        }
    }
}
