using CRIS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechCLI.Interfaces
{
    public interface ITranscriptParser
    {
        string Parse(TranscriptionResult transcriptionResult, out string outputExtension);
    }
}
