using System;
using System.Collections.Generic;
using System.Text;

namespace CustomSpeechCLI
{
    public class Config
    {
        public static readonly string CONFIG_FILENAME = "config.json";

        public string Name { get; set; }
        public string SpeechKey { get; set; }
        public string SpeechRegion { get; set; }
        public bool Selected { get; set; }

        public const string SUCCEEDED_STATUS = "Succeeded";
        public const string FAILED_STATUS = "Failed";

        public Config() { }

        public Config(string defaultName = "", string defaultKey = "", string defaultRegion = "", bool defaultSelected = false)
        {
            Name = defaultName;
            SpeechKey = defaultKey;
            SpeechRegion = defaultRegion;
        }
    }
}
