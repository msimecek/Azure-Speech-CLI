using CRIS.Models;
using Newtonsoft.Json;
using SpeechCLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechCLI.Utils
{
    public class JsonTranscriptParser : ITranscriptParser
    {
        public (string text, string extension) Parse(TranscriptionResult transcriptionResult)
        {
            return (JsonConvert.SerializeObject(transcriptionResult, Formatting.Indented), ".json");
        }
    }
}
