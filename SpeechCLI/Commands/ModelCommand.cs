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
    [Command(Description = "Acoustic model management.")]
    [Subcommand("create", typeof(Create))]
    [Subcommand("list", typeof(List))]
    [Subcommand("list-scenarios", typeof(ListScenarios))]
    [Subcommand("status", typeof(Status))]
    [Subcommand("delete", typeof(Delete))]
    [Subcommand("locales", typeof(Locales))]
    class ModelCommand : SpeechCommandBase
    {
        public ModelCommand(ISpeechServicesAPIv20 speechApi, IConsole console) : base(speechApi, console, null) { }

        [Command(Description = "Create new acoustic model based on adaptation dataset.\nYou will need dataset ID and scenario ID (run 'model list-scenarios' first).")]
        class Create : ParamActionCommandBase
        {
            [Option(Description = "(Required) Acoustic model name.")]
            [Required]
            string Name { get; set; }

            [Option(Description = "Acoustic model description.")]
            string Description { get; set; }

            [Option(Description = "Locale of the model. Must be the same as acoustic dataset locale. Default: en-us.")]
            string Locale { get; set; }

            [Option(ValueName = "GUID", Description = "ID of the adaptation dataset used to train the model. Required when '--language-dataset' not provided.")]
            [Guid]
            string AudioDataset { get; set; }

            [Option(ShortName = "lng", ValueName = "GUID", Description = "ID of the language dataset used to train language model. Required when '--audio-dataset' not provided.")]
            [Guid]
            string LanguageDataset { get; set; }

            [Option(ShortName = "pro", ValueName = "GUID", Description = "ID of the pronunciation dataset used to train language model. Required when '--audio-dataset' not provided.")]
            [Guid]
            string PronunciationDataset { get; set; }

            [Option(ValueName = "GUID", Description = "(Required) ID of base acoustic model. To get available scenarios for given locale, run 'model list-scenarios --locale en-us'.")]
            [Guid]
            [Required]
            string Scenario { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Will stop and wait for training to complete.")]
            bool Wait { get; set; }

            [Option(Description = "Custom properties of this model. Format: '--properties prop1=val1;prop2=val2;prop3=val3'")]
            string Properties { get; set; }

            int OnExecute()
            {
                if (string.IsNullOrWhiteSpace(AudioDataset) 
                    && string.IsNullOrWhiteSpace(LanguageDataset) 
                    && string.IsNullOrWhiteSpace(PronunciationDataset))
                {
                    _console.Error.WriteLine("Either --audio-dataset or --language-dataset has to be provided.");
                    return -1;
                }

                var modelDefinition = new ModelDefinition()
                {
                    BaseModel = new ModelIdentity(Guid.Parse(Scenario)),
                    Locale = Locale ?? "en-us",
                    Description = Description,
                    Name = Name,
                    Properties = SplitProperties(Properties),
                };

                if (!string.IsNullOrWhiteSpace(AudioDataset))
                {
                    modelDefinition.ModelKind = "Acoustic";
                    modelDefinition.Datasets = new List<DatasetIdentity>() { new DatasetIdentity(Guid.Parse(AudioDataset)) };
                }
                else
                {
                    modelDefinition.ModelKind = "Language";
                    if (string.IsNullOrWhiteSpace(PronunciationDataset))
                    {
                        modelDefinition.Datasets = new List<DatasetIdentity>() { new DatasetIdentity(Guid.Parse(LanguageDataset)) };
                    } else
                    {
                        modelDefinition.Datasets = new List<DatasetIdentity>() { new DatasetIdentity(Guid.Parse(LanguageDataset)),
                                                                                 new DatasetIdentity(Guid.Parse(PronunciationDataset))};
                    }

                }

                _console.WriteLine("Creating model...");

                var res = CreateAndWait(() => _speechApi.CreateModel(modelDefinition), Wait, _speechApi.GetModel);

                return res;
            }
        }

        [Command(Description = "Lists acoustic and language models in your subscription.")]
        class List
        {
            int OnExecute()
            {
                _console.WriteLine("Getting models...");

                var res = CallApi<List<Model>>(_speechApi.GetModels);
                if (res == null)
                    return -1;

                if (res.Count == 0)
                {
                    _console.WriteLine("No models found.");
                }
                else
                {
                    foreach (var m in res.Where(m => m.BaseModel != null).OrderBy(m => m.ModelKind))
                    {
                        _console.WriteLine($"{m.Id,30} {m.Name,25} {m.ModelKind} {m.Status}");
                    }
                }

                return 0;
            }
        }

        [Command(Description = "Lists scenario models (base models) for given locale.")]
        class ListScenarios
        {
            [Option(Description = "Language of the model. Default: en-us.")]
            string Locale { get; set; }

            [Option(Description = "Purpose of the base model. Use 'all' to disable purpose filter. Default: AcousticAdaptation.")]
            string Purpose { get; set; } = "AcousticAdaptation";

            [Option(CommandOptionType.NoValue, Description = "Returns only a list of GUIDs, without additional information. Ordered from newest to oldest.")]
            bool Simple { get; set; }

            int OnExecute()
            {
                Locale = Locale ?? "en-us";

                if (!Simple)
                    _console.WriteLine($"Getting scenarios for {Locale}...");

                var res = CallApi<List<Model>>(_speechApi.GetModels);
                if (res == null)
                    return -1;

                if (res.Count == 0)
                {
                    _console.WriteLine("No scenario models found.");
                }
                else
                {
                    foreach (var m in res
                        .Where(
                            m => m.Locale.ToLower() == Locale.ToLower() && 
                            m.BaseModel == null && 
                            (Purpose.ToLower() == "all" ? true : m.Properties["Purpose"].Contains(Purpose)))
                        .OrderByDescending(m => m.CreatedDateTime))
                    {
                        _console.WriteLine(
                            Simple ? 
                                $"{m.Id}" : 
                                $"{m.Id,30} {m.Name,30} {m.CreatedDateTime}");
                    }
                }

                return 0;
            }
        }

        [Command(Description = "Shows status of specific acoustic or language model.")]
        class Status : ParamActionCommandBase
        {
            [Argument(0, Name = "GUID", Description = "(Required) Model ID.")]
            [Required]
            [Guid]
            string Id { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Returns just simple representation of status. For example: Succeeded.")]
            bool Simple { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Getting model...");

                var res = CallApi<Model>(() => _speechApi.GetModel(Guid.Parse(Id)));
                if (res == null)
                    return -1;

                if (Simple == true)
                    _console.WriteLine(res.Status);
                else
                    _console.WriteLine($"{res.Id,30} {res.Name,-25} {res.Status}");

                return 0;
            }
        }

        [Command(Description = "Delete specific acoustic or language model.")]
        class Delete : ParamActionCommandBase
        {
            [Argument(0, Name = "GUID", Description = "(Required) ID of the model to delete.")]
            [Required]
            [Guid]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Deleting model...");

                CallApi<ErrorContent>(() => _speechApi.DeleteModel(Guid.Parse(Id)));
                _console.WriteLine("Done.");

                return 0;
            }
        }

        [Command(Description = "List locales available to create models.")]
        class Locales : ParamActionCommandBase
        {
            [Argument(0, Name = "acoustic|language", Description = "Type of models.")]
            [Required]
            [Enum(ACOUSTIC, LANGUAGE)]
            string Type { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Returns only a list of locales, without additional information.")]
            bool Simple { get; set; }

            int OnExecute()
            {
                var locales = CallApi<GetSupportedLocalesForModelsOKResponse>(_speechApi.GetSupportedLocalesForModels);

                var results = new Dictionary<string, IList<string>>()
                {
                    { ACOUSTIC, locales.Acoustic },
                    { LANGUAGE, locales.Language },
                    //{ "customvoice", locales.CustomVoice }, // none available at the moment
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
                            _console.WriteLine($"Supported locales for {Type.ToLowerInvariant()} models:");

                        _console.WriteLine(string.Join('\n', results[Type.ToLowerInvariant()]));
                    }

                    return 0;
                }

                return -1;
            }
        }
    }
}
