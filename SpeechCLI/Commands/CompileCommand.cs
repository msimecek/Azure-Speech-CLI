using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SpeechCLI.Commands
{
    [Command(Description = "Helper to compile WAV samples and transcripts.")]
    class CompileCommand
    {
        [Option(LongName = "audio", ShortName = "a", ValueName = "FOLDER", Description = "Path to folder where all WAV files are.")]
        [Required]
        [DirectoryExists]
        string AudioPath { get; set; }

        [Option(LongName = "transcript", ShortName = "t", ValueName = "FILE", Description = "Path to the TXT file with transcripts. One line per WAV file.")]
        [Required]
        [FileExists]
        string TranscriptPath { get; set; }

        [Option(LongName = "output", ShortName = "o", ValueName = "FOLDER", Description = "Target folder. Will be created if non-existent. Default: audio folder")]
        string OutputPath { get; set; }

        [Option(LongName = "test-percentage", ShortName = "tp", ValueName = "PERCENTAGE", Description = "What portion (in %) of source data will be split as test dataset. Default: 10")]
        int? TestPercentage { get; set; }

        [Option(CommandOptionType.NoValue, Description = "Delete training and testing WAV files when done and keep only ZIPped results.")]
        bool? Clean { get; set; }

        int OnExecute(IConsole console)
        {
            console.WriteLine("Compiling files...");

            var files = Directory.GetFiles(AudioPath).OrderBy(f => f).ToArray();
            var lines = File.ReadAllLines(TranscriptPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray(); // not interested in empty lines

            if (files.Length == 0 || lines.Length == 0)
            {
                console.Error.WriteLine("No source files found or transcript file empty.");
                return -1;
            }

            //TODO: check for BOM, add BOM if missing
            //TODO: skip lines which don't have a corresponding WAV file

            var numberOfTests = (int)Math.Round((double)(files.Length * (TestPercentage ?? 10) / 100));
            console.WriteLine($"Found {files.Length} files and {lines.Length} text lines. {numberOfTests} will be used as test dataset.");

            var outputFolder = OutputPath ?? Path.Join(AudioPath, "Output");
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            try
            {
                var trainFiles = files.Take(files.Length - numberOfTests);
                var trainLines = lines.Take(lines.Length - numberOfTests);
                var trainFolder = Path.Join(outputFolder, "Train");

                console.WriteLine("Copying and ZIPing training files.");
                CreateCopyAndZip(trainFolder, trainFiles, trainLines, outputFolder, "Train.zip", "train.txt");

                if (Clean != null && Clean == true)
                    Directory.Delete(trainFolder, true);

                if (numberOfTests > 0)
                {
                    var testFiles = files.Reverse().Take(numberOfTests);
                    var testLines = lines.Reverse().Take(numberOfTests);
                    var testFolder = Path.Join(outputFolder, "Test");

                    console.WriteLine("Copying and ZIPing testing files.");
                    CreateCopyAndZip(testFolder, testFiles, testLines, outputFolder, "Test.zip", "test.txt");

                    if (Clean != null && Clean == true)
                        Directory.Delete(testFolder, true);
                }
            }
            catch(Exception ex)
            {
                console.Error.WriteLine("Error: " + ex.Message);
                return -1;
            }

            console.WriteLine("Done.");
            return 0;
        }

        void CreateCopyAndZip(string folder, IEnumerable<string> audioFiles, IEnumerable<string> audioLines, string outputFolder, string zipFileName, string txtFileName)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var f in audioFiles)
                File.Copy(f, Path.Combine(folder, Path.GetFileName(f)), true);

            ZipFile.CreateFromDirectory(folder, Path.Join(outputFolder, zipFileName), CompressionLevel.Fastest, false);
            File.WriteAllLines(Path.Join(outputFolder, txtFileName), audioLines);
        }

    }
}
