using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeechCLI.Commands
{
    /// <summary>
    /// Used with commands which require parameters. Shows help when no params provided, otherwise shows error.
    /// </summary>
    abstract class ParamActionCommandBase
    {
        protected void OnValidationError(System.ComponentModel.DataAnnotations.ValidationResult r, CommandLineApplication app)
        {
            if (app.GetOptions().All(o => !o.HasValue()))
            {
                app.ShowHelp();
            }
            else
            {
                Console.Error.WriteLine(r.ErrorMessage);
            }
        }
    }
}
