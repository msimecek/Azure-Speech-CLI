using CRIS;
using CRIS.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SpeechCLI.Commands
{
    [HelpOption("--help")]
    abstract class SpeechCommandBase
    {
        protected static ISpeechServicesAPIv20 _speechApi;
        protected static IConsole _console;

        public SpeechCommandBase(ISpeechServicesAPIv20 speechApi, IConsole console)
        {
            _speechApi = speechApi;
            _console = console;
        }

        protected void OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
        }

        /// <summary>
        /// Call API with method returning null for success and ErrorContent for failure.
        /// </summary>
        protected static int CallApi(Func<ErrorContent> method)
        {
            var res = method.Invoke();
            if (res != null)
            {
                _console.Error.WriteLine($"API call ended with error: {res.Message}");
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Call API with method returning meaningful object for success and ErrorContent for failure.
        /// </summary>
        protected static T CallApi<T>(Func<object> method)
        {
            var res = method.Invoke();
            if (res != null && res is ErrorContent)
            {
                _console.Error.WriteLine($"API call ended with error: {(res as ErrorContent).Message}");
                return default(T);
            }

            return (T)res;
        }

        protected static int CreateAndWait(Func<object> operation, bool wait, Func<Guid, object> probe)
        {
            var res = operation.Invoke();
            if (res is Guid)
            {
                if (wait)
                    return WaitForProcessing((Guid)res, probe);
                else
                    _console.WriteLine("Created.");
                return 0;
            }
            else
            {
                _console.WriteLine("There was an error while creating: " + ((ErrorContent)res).Message);
                return -1;
            }
        }

        protected static int WaitForProcessing(Guid id, Func<Guid, object> probe)
        {
            _console.Write("Processing [.");
            var done = false;
            while (!done)
            {
                _console.Write(".");
                Thread.Sleep(1000);
                var resource = probe.Invoke(id);
                if (resource is Entity)
                {
                    if ((resource as Entity).Status == Config.FAILED_STATUS)
                    {
                        _console.WriteLine(".]");
                        _console.Error.WriteLine("Processing failed.");
                        return -1;
                    }

                    done = (resource as Entity).Status == Config.SUCCEEDED_STATUS;
                }
                else
                {
                    // přišel ErrorResult nebo něco jiného
                    _console.Error.WriteLine("Unable to get status. " + (resource as ErrorContent).Message);
                    return -1;
                }

            }
            _console.WriteLine(".] Done");
            _console.WriteLine(id.ToString());
            return 0;
        }
    }
}
