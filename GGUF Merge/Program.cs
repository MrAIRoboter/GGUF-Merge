using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GGUFMerge.Utils.Extensions;
using Utils;

namespace GGUFMerge
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
                Logger.Log("Enter path to the any split file (e.g. *.gguf-split-a): ");
                splitFilePath = Console.ReadLine() ?? string.Empty;
            }

            Logger.Log($"Selected file path > '{splitFilePath}'", false);

            splitFilePath = splitFilePath.Replace("\"", "");
            splitFilePath = Path.GetFullPath(splitFilePath);

            if (File.Exists(splitFilePath) == false)
            {
                Logger.Log($"File path does not exist - {splitFilePath}");
                return;
            }

            string targetDirectory = Path.GetDirectoryName(splitFilePath) ?? string.Empty;
            List<string> orderedRelatedFilePaths = GetAllRelatedFilePaths(splitFilePath, targetDirectory).OrderBy(filePath => Path.GetFileName(filePath)).ToList();
            string firstSplitFilePath = orderedRelatedFilePaths.First();
            FileInfo firstSplitFileInfo = new FileInfo(firstSplitFilePath);

            Console.WriteLine();
            Logger.Log("The files will be combined in the following sequence: ");

            for (int i = 0; i < orderedRelatedFilePaths.Count; i++)
                Logger.Log($"{i + 1}/{orderedRelatedFilePaths.Count} | {Path.GetFileName(orderedRelatedFilePaths[i])}");

            Stopwatch stopwatch = Stopwatch.StartNew();
            long lastMeasuredTime = 0;

            Console.WriteLine();
            Logger.Log("The appending process has started. Don't close the program!");

            for (int i = 1; i < orderedRelatedFilePaths.Count; i++)
            {
                string filePath = orderedRelatedFilePaths[i];
                FileInfo appendFileInfo = new FileInfo(filePath);

                firstSplitFileInfo.AppendFile(appendFileInfo);
                File.Delete(filePath);

                Logger.Log($"{i}/{orderedRelatedFilePaths.Count - 1} | File {filePath} - attached ({stopwatch.ElapsedMilliseconds - lastMeasuredTime} ms.)");

                lastMeasuredTime = stopwatch.ElapsedMilliseconds;
            }

            RenameFileWithNewExtension(firstSplitFilePath, ".gguf");

            Console.WriteLine();
            Logger.Log($"All files are attached! ({stopwatch.ElapsedMilliseconds} ms.)");
        }

        private static void RenameFileWithNewExtension(string filePath, string newExtension)
        {
            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

            string newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}");

            if (Path.GetExtension(fileNameWithoutExtension).ToLower() != newExtension.ToLower())
                newFilePath += newExtension;

            File.Move(filePath, newFilePath);
        }

        private static List<string> GetAllRelatedFilePaths(string filePath, string directory)
        {
            List<string> relatedFilePaths = new List<string>();
            List<string> allFilePathsInDirectory = Directory.GetFiles(directory).ToList();

            foreach (string path in allFilePathsInDirectory)
                if (path.GetSubstringBeforeLastSymbol('-') == filePath.GetSubstringBeforeLastSymbol('-'))
                    relatedFilePaths.Add(path);

            return relatedFilePaths;
        }
    }
}