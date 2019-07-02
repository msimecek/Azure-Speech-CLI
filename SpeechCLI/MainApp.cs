using McMaster.Extensions.CommandLineUtils;
using SpeechCLI.Commands;

namespace SpeechCLI
{
    [Command(Name = "speech", Description = "Command-line interface for Azure Speech service.")]
    [Subcommand("config", typeof(ConfigCommand))]
    [Subcommand("dataset", typeof(DatasetCommand))]
    [Subcommand("model", typeof(ModelCommand))]
    [Subcommand("test", typeof(TestCommand))]
    [Subcommand("endpoint", typeof(EndpointCommand))]
    [Subcommand("compile", typeof(CompileCommand))]
    [Subcommand("transcript", typeof(TranscriptCommand))]
    public class MainApp
    {
        int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return 0;
        }
    }
}
