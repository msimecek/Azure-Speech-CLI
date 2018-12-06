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

        /// <summary>
        /// Converts a string with properties to a key-value Dictionary.
        /// </summary>
        /// <param name="properties">Properties string in the form of: "name1=value1;name2=value2...".</param>
        protected static Dictionary<string, string> SplitProperties(string properties)
        {
            if (string.IsNullOrWhiteSpace(properties))
                return null;

            var res = new Dictionary<string, string>();
            var props = properties.Split(';');
            foreach (string prop in props)
            {
                var split = prop.Split('=');
                if (split.Length == 2)
                {
                    res.Add(split[0], split[1]);
                }
            }

            return res;
        }
    }
}
