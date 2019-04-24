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
        public string Parse(TranscriptionResult transcriptionResult, out string outputExtension)
        {
            outputExtension = ".json";
            return JsonConvert.SerializeObject(transcriptionResult);
        }
    }
}
