using SpeechCLI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpeechCLI
{
    public class Config : IConfig
    {
        public static readonly string CONFIG_FILENAME = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "/.speech/config.json");

        public string Name { get; set; }
        public string SpeechKey { get; set; }
        public string SpeechRegion { get; set; }
        public bool Selected { get; set; }

        public Config() { }

        public Config(string defaultName = "", string defaultKey = "", string defaultRegion = "", bool defaultSelected = false)
        {
            Name = defaultName;
            SpeechKey = defaultKey;
            SpeechRegion = defaultRegion;
        }
    }
}
