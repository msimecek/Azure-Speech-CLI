using CRIS;
using CRIS.Models;
using SpeechCLI.Attributes;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpeechCLI.Commands
{
    [Command(Description = "Commands to manage adaptation datasets.")]
    [Subcommand("create", typeof(Create))]
    [Subcommand("list", typeof(List))]
    [Subcommand("show", typeof(Show))]
    [Subcommand("delete", typeof(Delete))]
    [Subcommand("locales", typeof(Locales))]
    class DatasetCommand : SpeechCommandBase
    {
        public DatasetCommand(ISpeechServicesAPIv20 speechApi, IConsole console) : base(speechApi, console, null) { }

        [Command(Description = "Create new acoustic dataset.\n- Provide --audio and --transcript file to create acoustic dataset.\n- Provide --language file to create language dataset.")]
        class Create : ParamActionCommandBase
        {
            [Option(Description = "(Required) Name of this data import.")]
            [Required]
            string Name { get; set; }

            [Option(Description = "Description of this data import.")]
            string Description { get; set; }

            [Option(Description = "Language of the data. Default: en-us")]
            string Locale { get; set; }

            [Option(ValueName = "FILE", Description = "ZIP file containing all audio samples. If provided, --transcript is also required.")]
            [FileExists]
            string Audio { get; set; }

            [Option(ValueName = "FILE", Description = "TXT file with transcriptions of audio files. Must be UTF-8 BOM. If provided, --audio is also required.")]
            [FileExists]
            string Transcript { get; set; }

            [Option(ShortName = "lng", LongName = "language", ValueName = "FILE", Description = "TXT file with language dataset.")]
            [FileExists]
            string Language { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Will stop and wait for dataset to be ready.")]
            bool Wait { get; set; }

            [Option(Description = "Custom properties of this dataset. Format: '--properties prop1=val1;prop2=val2;prop3=val3'")]
            string Properties { get; set; }

            int OnExecute()
            {
                var res = 0;

                if (Properties != null)
                {
                    Properties = SafeJsonConvert.SerializeObject(SplitProperties(Properties)); // transform to the right format for upload
                }

                if (Audio != null && Transcript != null)
                {
                    var audiodataFile = System.IO.File.OpenRead(Audio);
                    var transcriptFile = System.IO.File.OpenRead(Transcript);

                    _console.WriteLine("Uploading acoustic dataset...");
                    res = CreateAndWait(
                        () => _speechApi.UploadDataset(Name, Description, Locale ?? "en-us", "Acoustic", audiodata: audiodataFile, transcriptions: transcriptFile, properties: Properties), 
                        Wait,
                        _speechApi.GetDataset);
                }

                if (Language != null)
                {
                    var languageFile = System.IO.File.OpenRead(Language);

                    _console.WriteLine("Uploading language dataset...");
                    res = CreateAndWait(
                        () => _speechApi.UploadDataset(Name, Description, Locale ?? "en-us", "Language", languagedata: languageFile, properties: Properties), 
                        Wait, 
                        _speechApi.GetDataset);
                }

                return res;
            }
        }

        [Command(Description = "Lists datasets in your subscription.")]
        class List
        {
            int OnExecute()
            {
                _console.WriteLine("Getting datasets...");

                var datasets = CallApi<List<Dataset>>(_speechApi.GetDatasets);
                if (datasets == null)
                    return -1;

                if (datasets.Count == 0)
                {
                    _console.WriteLine("No datasets found.");
                }
                else
                {
                    foreach (var d in datasets.OrderBy(m => m.DataImportKind))
                    {
                        _console.WriteLine($"{d.Id, -30} {d.Name, -25} {d.CreatedDateTime, 20} {d.Locale, 5} {d.DataImportKind, 10} {d.Status}");
                    }
                }

                return 0;
            }
            
        }

        [Command(Description = "Show details of a specific dataset.")]
        class Show : ParamActionCommandBase
        {
            [Argument(0, Name = "GUID", Description = "ID of the dataset to show.")]
            [Guid]
            [Required]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Getting dataset...");

                var res = CallApi<Dataset>(() => _speechApi.GetDataset(Guid.Parse(Id)));

                if (res == null)
                {
                    return -1;
                }

                _console.WriteLine(SafeJsonConvert.SerializeObject(res, new JsonSerializerSettings() { Formatting = Formatting.Indented }));

                return 0;
            }
        }

        [Command(Description = "Delete specific dataset.")]
        class Delete : ParamActionCommandBase
        {
            [Argument(0, Name = "GUID", Description = "ID of the dataset to delete.")]
            [Required]
            [Guid]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Deleting dataset...");

                CallApi<ErrorContent>(() => _speechApi.DeleteDataset(Guid.Parse(Id)));
                _console.WriteLine("Done.");

                return 0;
            }
        }

        [Command(Description = "List locales available to create datasets.")]
        class Locales : ParamActionCommandBase
        {
            [Argument(0, Name = "acoustic|language|pronounciation", Description = "Type of datasets.")]
            [Required]
            [Enum(ACOUSTIC, LANGUAGE, PRONOUNCIATION)]
            string Type { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Returns only a list of locales, without additional information.")]
            bool Simple { get; set; }

            int OnExecute()
            {
                var locales = CallApi<GetSupportedLocalesForDatasetsOKResponse>(_speechApi.GetSupportedLocalesForDatasets);

                var results = new Dictionary<string, IList<string>>()
                {
                    { ACOUSTIC, locales.Acoustic },
                    { LANGUAGE, locales.Language },
                    //{ "customvoice", locales.CustomVoice }, // none available at the moment
                    //{ "languagegeneration", locales.LanguageGeneration }, // none available at the moment
                    { PRONOUNCIATION, locales.Pronunciation },
                };

                if (results.ContainsKey(Type.ToLowerInvariant()))
                {
                    if (results[Type.ToLowerInvariant()] == null)
                    {
                        if (!Simple)
                            _console.WriteLine("No locales for this type.");
                    }
                    else
                    {
                        if (!Simple)
                            _console.WriteLine($"Supported locales for {Type.ToLowerInvariant()} datasets:");

                        _console.WriteLine(string.Join('\n', results[Type.ToLowerInvariant()]));
                    }

                    return 0;
                }

                return -1;
            }
        }
    }
}
