using CRIS;
using SpeechCLI.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace SpeechCLI
{
    [Command(Name = "speech", Description = "Command-line interface for Azure Speech service.")]
    [Subcommand("config", typeof(ConfigCommand))]
    [Subcommand("dataset", typeof(DatasetCommand))]
    [Subcommand("model", typeof(ModelCommand))]
    [Subcommand("test", typeof(TestCommand))]
    [Subcommand("endpoint", typeof(EndpointCommand))]
    [Subcommand("compile", typeof(CompileCommand))]
    [Subcommand("batch", typeof(BatchCommand))]
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication<Program> app = new CommandLineApplication<Program>();
            app.HelpOption();
            app.VersionOptionFromAssemblyAttributes(typeof(Program).Assembly);

            if (!File.Exists(Config.CONFIG_FILENAME))
            {
                Console.WriteLine("No configuration found. Please run 'config set' first.");
                app.Conventions.UseDefaultConventions();
            }
            else
            {
                var config = SafeJsonConvert.DeserializeObject<List<Config>>(File.ReadAllText(Config.CONFIG_FILENAME)).FirstOrDefault(c => c.Selected == true);
                if (config == null)
                {
                    Console.WriteLine("No configuration set selected. Run 'config select --name <config set name>' to choose one.");
                    app.Conventions.UseDefaultConventions();
                }
                else
                {
                    var hc = new HttpClient();
                    hc.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config.SpeechKey);
                    var sdk = new SpeechServicesAPIv20(hc, true);
                    sdk.BaseUri = new Uri($"https://{config.SpeechRegion}.cris.ai");

                    var services = new ServiceCollection()
                        .AddSingleton<ISpeechServicesAPIv20>(sdk)
                        .BuildServiceProvider();

                    app.Conventions.UseDefaultConventions().UseConstructorInjection(services);
                }
            }

            try
            {
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return 0;
        }
        
    }
}
