﻿using CRIS;
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
    class ModelCommand : SpeechCommandBase
    {
        public ModelCommand(ISpeechServicesAPIv20 speechApi, IConsole console) : base(speechApi, console) { }

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
                if (string.IsNullOrWhiteSpace(AudioDataset) && string.IsNullOrWhiteSpace(LanguageDataset))
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
                    modelDefinition.Datasets = new List<DatasetIdentity>() { new DatasetIdentity(Guid.Parse(LanguageDataset)) };
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

            int OnExecute()
            {
                _console.WriteLine("Getting scenarios...");

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
                        .Where(m => m.Locale.ToLower() == (Locale ?? "en-us").ToLower() && m.BaseModel == null)
                        .OrderBy(m => m.ModelKind))
                    {
                        _console.WriteLine($"{m.Id,30}\t{m.Name}");
                    }

                }

                return 0;
            }
        }

        [Command(Description = "Shows status of specific acoustic or language model.")]
        class Status : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "(Required) Model ID.")]
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
            [Option(ValueName = "GUID", Description = "(Required) ID of the model to delete.")]
            [Required]
            [Guid]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Deleting model...");

                var res = CallApi(() => _speechApi.DeleteModel(Guid.Parse(Id)));
                _console.WriteLine("Done.");

                return res;
            }
        }
    }
}
