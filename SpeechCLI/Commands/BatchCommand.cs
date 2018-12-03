using CRIS;
using CRIS.Models;
using CustomSpeechCLI.Attributes;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Rest.Serialization;
using SpeechCLI.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpeechCLI.Commands
{
    [Command(Description = "Batch transcription.")]
    [Subcommand("create", typeof(Create))]
    [Subcommand("list", typeof(List))]
    [Subcommand("show", typeof(Show))]
    //[Subcommand("status", typeof(Status))]
    [Subcommand("delete", typeof(Delete))]
    [Subcommand("download", typeof(Download))]
    class BatchCommand : SpeechCommandBase
    {
        public BatchCommand(ISpeechServicesAPIv20 speechApi, IConsole console) : base(speechApi, console) { }

        [Command(Description = "Start new batch transcription.")]
        class Create : ParamActionCommandBase
        {
            [Option(Description = "(Required) Name of this transcription.")]
            [Required]
            string Name { get; set; }

            [Option(Description = "Description of this transcription.")]
            string Description { get; set; }

            [Option(Description = "Language. Default: en-us")]
            string Locale { get; set; }

            [Option(ValueName = "URL", Description = "URL pointing to audio file.")]
            string Recording { get; set; }

            [Option(ValueName = "GUID", Description = "ID of the acoustic model to use. Run 'model list' to get your models. Default: baseline.")]
            [Guid]
            string Model { get; set; }

            [Option(ShortName = "lng", LongName = "language", ValueName = "GUID", Description = "ID of language model to use. Run 'model list' to get your models. Default: baseline.")]
            string LanguageModel { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Will stop and wait for transcription to be ready.")]
            bool Wait { get; set; }

            int OnExecute()
            {
                var definition = new TranscriptionDefinition(Recording, Locale ?? "en-us", Name);
                definition.ModelsProperty = new List<ModelIdentity>();

                if (!string.IsNullOrEmpty(Model))
                    definition.ModelsProperty.Add(new ModelIdentity(Guid.Parse(Model)));

                if (!string.IsNullOrEmpty(LanguageModel))
                    definition.ModelsProperty.Add(new ModelIdentity(Guid.Parse(LanguageModel)));

                _console.WriteLine("Creating transcript...");
                var res = CreateAndWait(
                    () => _speechApi.CreateTranscription(definition), 
                    Wait, 
                    _speechApi.GetTranscription);

                return res;
            }
        }

        [Command(Description = "Get a list of batch transcriptions.")]
        class List
        {
            int OnExecute()
            {
                _console.WriteLine("Getting transcriptions...");
                _console.WriteLine();

                var res = CallApi<List<Transcription>>(_speechApi.GetTranscriptions);
                if (res == null)
                    return -1;

                if (res.Count == 0)
                {
                    _console.WriteLine("No transcriptions found.");
                }
                else
                {
                    foreach (var t in res)
                    {
                        _console.WriteLine($"{t.Id,30} {t.Name} {t.Status}");
                    }
                }

                return 0;
            }
        }

        [Command(Description = "Show specific batch transcription.")]
        class Show : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "ID of transcription. Use 'batch list' to get your transcriptions.")]
            [Guid]
            [Required]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Getting transcription...");
                _console.WriteLine();

                var res = CallApi<Transcription>(() => _speechApi.GetTranscription(Guid.Parse(Id)));
                if (res == null)
                    return -1;

                _console.WriteLine(SafeJsonConvert.SerializeObject(res, new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented }));

                return 0;
            }
        }

        [Command(Description = "Delete specific batch transcription.")]
        class Delete : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "ID of the transcription to delete.")]
            [Required]
            [Guid]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Deleting transcription...");
                var res = CallApi(() => _speechApi.DeleteTranscription(Guid.Parse(Id)));
                _console.WriteLine("Done.");

                return res;
            }
        }

        [Command(Description = "Download results of finished transcription.")]
        class Download : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "ID of the transcription to download.")]
            [Required]
            [Guid]
            string Id { get; set; }

            [Option(Description = "Output directory where transcriptions will be downloaded.")]
            [Required]
            [LegalFilePath]
            string OutDir { get; set; }

            [Option(ValueName = "JSON|VTT", Description = "(Optional) Output format. Currently supported are: VTT and JSON. Default = JSON")]
            string Format { get; set; }

            async Task<int> OnExecute()
            {
                // get transcript
                _console.WriteLine("Getting transcription...");
                var transcription = CallApi<Transcription>(() => _speechApi.GetTranscription(Guid.Parse(Id)));

                // read URLs (channels)
                foreach (var url in transcription.ResultsUrls)
                {
                    var output = "WEBVTT" + Environment.NewLine + Environment.NewLine;

                    using (var hc = new HttpClient())
                    {
                        // download URLs
                        var res = await hc.GetAsync(url.Value);
                        if (res.IsSuccessStatusCode)
                        {
                            // parse
                            var rawContent = await res.Content.ReadAsStringAsync();
                            var convertedContent = SafeJsonConvert.DeserializeObject<TranscriptionResult>(rawContent);
                            var outputFileName = Path.Join(OutDir, convertedContent.AudioFileResults[0].AudioFileName);

                            switch(Format?.ToLower())
                            {
                                case "vtt":
                                    outputFileName += ".vtt";
                                    var segments = convertedContent.AudioFileResults[0].SegmentResults;
                                    foreach (var result in segments)
                                    {
                                        if (result.RecognitionStatus == "Success")
                                        {
                                            var best = result.NBest[0];
                                            var startTime = TimeSpan.FromTicks(result.Offset).ToString(@"hh\:mm\:ss\.fff");
                                            var endTime = TimeSpan.FromTicks(result.Offset + result.Duration).ToString(@"hh\:mm\:ss\.fff");

                                            output += $"{startTime} --> {endTime}" + Environment.NewLine;

                                            output += (best.Confidence > 0.5) ? best.Display : "";
                                            output += Environment.NewLine + Environment.NewLine;
                                        }
                                    }
                                    break;
                                case "json":
                                default:
                                    outputFileName += ".json";
                                    output = rawContent;
                                    break;
                            }

                            // save to output
                            File.WriteAllText(outputFileName, output);
                            _console.WriteLine($"File {outputFileName} written.");
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }

                return 0;
            }

        }
    }
}
