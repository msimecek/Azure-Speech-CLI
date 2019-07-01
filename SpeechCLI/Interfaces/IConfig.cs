using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechCLI.Interfaces
{
    public interface IConfig
    {
        string Name { get; set; }
        string SpeechKey { get; set; }
        string SpeechRegion { get; set; }
        bool Selected { get; set; }
    }
}
