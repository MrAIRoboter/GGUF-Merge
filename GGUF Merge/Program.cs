using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utils;

namespace GGUF_Merge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string splitFilePath = "";

            if (args.Length > 0)
            {
                splitFilePath = args[0];
            }
            else
            {
                Logger.Log("Enter path to the any split file (*.gguf-split-a): ");
                splitFilePath = Console.ReadLine();
            }

            Logger.Log($"Selected file path > '{splitFilePath}'", false);

            splitFilePath = splitFilePath.Replace("\"", "");
            splitFilePath = Path.GetFullPath(splitFilePath);

            if (File.Exists(splitFilePath) == false)
            {
                Logger.Log($"File path does not exist - {splitFilePath}");
                return;
            }

            string targetDirectory = Path.GetDirectoryName(splitFilePath);
            List<string> orderedRelatedFilePaths = GetAllRelatedFilePaths(splitFilePath, targetDirectory).OrderBy(filePath => filePath).ToList();
            string firstSplitFilePath = orderedRelatedFilePaths.First();
            Stopwatch stopwatch = Stopwatch.StartNew();
            long lastMeasuredTime = 0;

            Logger.Log("The appending process has started. Don't close the program!");

            for (int i = 1; i < orderedRelatedFilePaths.Count; i++)
            {
                string filePath = orderedRelatedFilePaths[i];

                AppendFile(firstSplitFilePath, filePath);
                File.Delete(filePath);

                Logger.Log($"{i}/{orderedRelatedFilePaths.Count - 1} | File {filePath} - attached ({stopwatch.ElapsedMilliseconds - lastMeasuredTime} ms.)");

                lastMeasuredTime = stopwatch.ElapsedMilliseconds;
            }

            RenameFileWithNewExtension(firstSplitFilePath, "gguf");

            Logger.Log($"All files are attached! ({stopwatch.ElapsedMilliseconds} ms.)");
        }

        private static void RenameFileWithNewExtension(string filePath, string newExtension)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}.{newExtension}");

            File.Move(filePath, newFilePath);
        }

        private static List<string> GetAllRelatedFilePaths(string filePath, string directory)
        {
            List<string> relatedFilePaths = new List<string>();
            List<string> allFilePathsInDirectory = Directory.GetFiles(directory).ToList();

            foreach (string path in allFilePathsInDirectory)
                if (GetSubstringBeforeLastSymbol(path, '-') == GetSubstringBeforeLastSymbol(filePath, '-'))
                    relatedFilePaths.Add(path);

            return relatedFilePaths;
        }

        private static string GetSubstringBeforeLastSymbol(string input, char symbol)
        {
            int lastIndex = input.LastIndexOf(symbol);
            return (lastIndex != -1) ? input.Substring(0, lastIndex) : input;
        }

        private static void AppendFile(string sourceFile, string targetFile)
        {
            using (FileStream sourceFileStream = new FileStream(sourceFile, FileMode.Append, FileAccess.Write))
            using (FileStream targetFileStream = new FileStream(targetFile, FileMode.Open, FileAccess.Read))
            {
                targetFileStream.CopyTo(sourceFileStream);
            }
        }
    }
}