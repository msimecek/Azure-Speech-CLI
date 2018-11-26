using CRIS;
using CRIS.Models;
using CustomSpeechCLI.Attributes;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using SpeechCLI.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;

namespace CustomSpeechCLI.Commands
{
    [Command(Description = "Commands to manage speech endpoints.")]
    [Subcommand("create", typeof(Create))]
    [Subcommand("list", typeof(List))]
    [Subcommand("show", typeof(Show))]
    [Subcommand("delete", typeof(Delete))]
    [Subcommand("delete-data", typeof(DeleteData))]
    class EndpointCommand : SpeechCommandBase
    {
        public EndpointCommand(ISpeechServicesAPIv20 speechApi, IConsole console) : base(speechApi, console) { }

        [Command(Description = "Create new endpoint model based on acoustic model.\nYou will need model ID (run 'speech model list' first).")]
        [HelpOption()]
        class Create : ParamActionCommandBase
        {
            [Option(Description = "(Required) Endpoint name.")]
            [Required]
            string Name { get; set; }

            [Option(Description = "Endpoint description.")]
            string Description { get; set; }

            [Option(Description = "Language of the endpoint. Must be the same as acoustic dataset locale. Default: en-us.")]
            string Locale { get; set; }

            [Option(LongName = "model", ShortName = "m", ValueName = "GUID", Description = "(Required) ID of the acoustic model. Run 'speech model list' to see your models.")]
            [Guid]
            [Required]
            string AcousticModel { get; set; }

            [Option(ShortName = "lm", ValueName = "GUID", Description = "(Required) ID of the lanuage model. Run 'speech model list' to see your models.")]
            [Guid]
            [Required]
            string LanguageModel { get; set; }

            [Option(Description = "The number of concurrent recognitions the endpoint supports. Default: 1.")]
            int? ConcurrentRecognitions { get; set; }

            [Option(ShortName = "cl", ValueName = "TRUEFALSE", Description = "A value indicating whether content logging (audio & transcriptions) is being used for a deployment. Default: true.")]
            bool? ContentLogging { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Will stop and wait for endpoint to be ready.")]
            bool Wait { get; set; }


            int OnExecute(CommandLineApplication app)
            {
                var endpointDefinition = new EndpointDefinition()
                {
                    Name = Name,
                    Description = Description,
                    Locale = Locale ?? "en-us",
                    ModelsProperty = new List<ModelIdentity>() { new ModelIdentity(Guid.Parse(AcousticModel)), new ModelIdentity(Guid.Parse(LanguageModel)) },
                    ConcurrentRecognitions = ConcurrentRecognitions ?? 1,
                    ContentLoggingEnabled = ContentLogging ?? true,
                };

                _console.WriteLine("Creating endpoint...");
                var res = CreateAndWait(() => _speechApi.CreateEndpoint(endpointDefinition), Wait, _speechApi.GetEndpoint);

                return res;
            }
        }

        [Command(Description = "Lists endpoints in your subscription.")]
        class List
        {
            int OnExecute()
            {
                _console.WriteLine("Getting endpoints...");
                _console.WriteLine();

                var res = CallApi<List<Endpoint>>(_speechApi.GetEndpoints);
                if (res == null)
                    return -1;

                if (res.Count == 0)
                {
                    _console.WriteLine("No endpoints found.");
                }
                else
                {
                    foreach (var m in res)
                    {
                        _console.WriteLine($"{m.Id,30} {m.Name,-25} {m.Status} {m.EndpointKind}");
                    }
                }

                return 0;
            }
        }

        [Command(Description = "Show details of a specific endpoint.")]
        class Show : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "ID of the endpoint to show.")]
            [Guid]
            [Required]
            string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Getting endpoint...");
                _console.WriteLine();

                var res = CallApi<Endpoint>(() => _speechApi.GetEndpoint(Guid.Parse(Id)));

                if (res == null)
                {
                    return -1;
                }

                _console.WriteLine(SafeJsonConvert.SerializeObject(res, new JsonSerializerSettings() { Formatting = Formatting.Indented }));

                return 0;
            }
        }

        [Command(Description = "Delete specific endpoint.")]
        class Delete : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "ID of the endpoint to delete.")]
            [Required]
            [Guid]
            string Id { get; set; }

            int OnExecute(IConsole console)
            {
                console.WriteLine("Deleting endpoint...");
                var res = CallApi(() => _speechApi.DeleteEndpoint(Guid.Parse(Id)));
                console.WriteLine("Done.");

                return res;
            }
        }

        [Command(Description = "Delete specific endpoint's data.")]
        class DeleteData : ParamActionCommandBase
        {
            [Option(ValueName = "GUID", Description = "ID of the endpoint whose data to delete.")]
            [Required]
            [Guid]
            string Id { get; set; }

            int OnExecute(IConsole console)
            {
                console.WriteLine("Deleting endpoint data...");
                var res = CallApi(() => _speechApi.DeleteEndpointData(Guid.Parse(Id)));
                console.WriteLine("Done.");

                return res;
            }
        }

    }
}
