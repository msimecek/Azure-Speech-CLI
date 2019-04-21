using CRIS;
using CRIS.Models;
using SpeechCLI.Attributes;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;

namespace SpeechCLI.Commands
{
    [Command(Description = "Commands related to acoustic model tests.")]
    [Subcommand("create", typeof(Create))]
    [Subcommand("list", typeof(List))]
    [Subcommand("show", typeof(Show))]
    [Subcommand("status", typeof(Status))]
    [Subcommand("delete", typeof(Delete))]
    class TestCommand : SpeechCommandBase
    {
        public TestCommand(ISpeechServicesAPIv20 speechApi, IConsole console) : base(speechApi, console) { }

        [Command(Description = "Creates new accuracy test.")]
        class Create : ParamActionCommandBase
        {
            [Option(Description = "(Required) Accuracy test name.")]
            [Required]
            string Name { get; set; }

            [Option(Description = "Accuracy test description.")]
            string Description { get; set; }

            [Option(ValueName = "GUID", Description = "(Required) ID of the adaptation dataset used for accuracy test (testing dataset). Run 'dataset list' to see your datasets.")]
            [Guid]
            [Required]
            string AudioDataset { get; set; }

            [Option(ValueName = "GUID", Description = "(Required) ID of the acoustic model against which to run the accuracy test. Run 'model list' to see your models.")]
            [Guid]
            [Required]
            string Model { get; set; }

            [Option(ShortName = "lm", ValueName = "GUID", Description = "(Required) ID of the lanuage model. Run 'speech model list' to see your models.")]
            [Guid]
            [Required]
            string LanguageModel { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Will stop and wait for testing to complete.")]
            bool Wait { get; set; }

            [Option(Description = "Custom properties of this test. Format: '--properties prop1=val1;prop2=val2;prop3=val3'")]
            string Properties { get; set; }

            int OnExecute()
            {
                var testDefinition = new TestDefinition()
                {
                    Dataset = new DatasetIdentity(Guid.Parse(AudioDataset)),
                    Description = Description,
                    ModelsProperty = new List<ModelIdentity>() { new ModelIdentity(Guid.Parse(Model)), new ModelIdentity(Guid.Parse(LanguageModel)) },
                    Name = Name,
                    Properties = SplitProperties(Properties),
                };

                _console.WriteLine("Creating test...");
                var res = CreateAndWait(() => _speechApi.CreateAccuracyTest(testDefinition), Wait, _speechApi.GetAccuracyTest);

                return res;
            }
        }

        [Command(Description = "Lists accuracy tests in your subscription.")]
        class List
        {
            int OnExecute()
            {
                _console.WriteLine("Getting accuracy tests...");

                var res = CallApi<List<Test>>(_speechApi.GetAccuracyTests);
                if (res == null)
                    return -1;

                if (res.Count == 0)
                {
                    _console.WriteLine("No tests found.");
                }
                else
                {
                    foreach (var test in res)
                    {
                        _console.WriteLine($"{test.Id, 30} {test.Name, -25} {test.Status} {test.WordErrorRate:P1}");
                    }
                }

                return 0;
            }
        }

        [Command(Description = "Shows details of specified accuracy test.")]
        class Show : ParamActionCommandBase
        {
            [Argument(0, Name = "GUID", Description = "(Required) ID of the test to show.")]
            [Guid]
            [Required]
            public string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Getting test...");

                var res = CallApi<Test>(() => _speechApi.GetAccuracyTest(Guid.Parse(Id)));
                if (res == null)
                    return -1;

                _console.WriteLine(SafeJsonConvert.SerializeObject(res, new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented }));

                return 0;
            }
        }

        [Command(Description = "Shows status of specific accuracy test.")]
        class Status : ParamActionCommandBase
        {
            [Argument(0, Name = "GUID", Description = "(Required) Accuracy test ID.")]
            [Required]
            [Guid]
            string Id { get; set; }

            [Option(CommandOptionType.NoValue, Description = "Returns just simple representation of status. For example: Succeeded.")]
            bool Simple { get; set; }

            int OnExecute()
            {
                var res = CallApi<Test>(() => _speechApi.GetAccuracyTest(Guid.Parse(Id)));
                if (res == null)
                    return -1;

                if (Simple == true)
                    _console.WriteLine(res.Status);
                else
                    _console.WriteLine($"{res.Id,30} {res.Name,-25} {res.Status}");

                return 0;
            }
        }

        [Command(Description = "Deletes specified accuracy test.")]
        class Delete : ParamActionCommandBase
        {
            [Argument(0, Name ="GUID", Description = "(Required) ID of the test to show.")]
            [Guid]
            [Required]
            public string Id { get; set; }

            int OnExecute()
            {
                _console.WriteLine("Deleting test...");
                var res = CallApi(() => _speechApi.DeleteAccuracyTest(Guid.Parse(Id)));
                _console.WriteLine("Done.");

                return res;
            }
        }

    }
}
