using CRIS;
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

    class Program
    {
        static void Main(string[] args)
        {
            var config = InitConfig();

            var hc = new HttpClient();
            hc.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config.SpeechKey);

            var sdk = new SpeechServicesAPIv20(hc, true);
            sdk.BaseUri = new Uri($"https://{config.SpeechRegion}.cris.ai");

            var services = InitServices(config, sdk);

            CommandLineApplication<MainApp> app = new CommandLineApplication<MainApp>();
            app.HelpOption();
            app.VersionOptionFromAssemblyAttributes(typeof(Program).Assembly);
            app.Conventions.UseDefaultConventions().UseConstructorInjection(services);

            try
            {
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.InnerException?.Message);
            }
        }

        static Config InitConfig()
        {
            if (!File.Exists(Config.CONFIG_FILENAME))
            {
                Directory.CreateDirectory(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".speech"));
                File.WriteAllText(Config.CONFIG_FILENAME, "[]");
            }

            var config = SafeJsonConvert.DeserializeObject<List<Config>>(File.ReadAllText(Config.CONFIG_FILENAME)).FirstOrDefault(c => c.Selected == true);
            if (config == null)
            {
                config = new Config("Anonymous", "", "northeurope");
            }

            return config;
        }

        static ServiceProvider InitServices(Config config, ISpeechServicesAPIv20 sdk)
        {
            var services = new ServiceCollection()
                .AddSingleton<Config>(config)
                .AddSingleton<ISpeechServicesAPIv20>(sdk)
                .BuildServiceProvider();

            return services;
        }
        
    }
}
