using CRIS.Models;
using SpeechCLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechCLI.Utils
{
    public class VttTranscriptParser : ITranscriptParser
    {
        public (string text, string extension) Parse(TranscriptionResult transcriptionResult)
        {
            var output = new StringBuilder("WEBVTT" + Environment.NewLine + Environment.NewLine);
            
            //var output = "WEBVTT" + Environment.NewLine + Environment.NewLine;

            var segments = transcriptionResult.AudioFileResults[0].SegmentResults;
            foreach (var result in segments)
            {
                if (result.RecognitionStatus == "Success")
                {
                    var best = result.NBest[0];
                    var startTime = TimeSpan.FromTicks(result.Offset).ToString(@"hh\:mm\:ss\.fff");
                    var endTime = TimeSpan.FromTicks(result.Offset + result.Duration).ToString(@"hh\:mm\:ss\.fff");

                    output.AppendLine($"{startTime} --> {endTime}");
                    output.AppendLine((best.Confidence > 0.5) ? best.Display : "");
                    output.AppendLine();

                    //output += (best.Confidence > 0.5) ? best.Display : "";
                    //output += Environment.NewLine + Environment.NewLine;
                }
            }

            return (output.ToString(), ".vtt");
        }
    }
}
