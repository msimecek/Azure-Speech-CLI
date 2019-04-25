using CRIS.Models;
using Newtonsoft.Json;
using SpeechCLI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SpeechCLI.Tests.UtilsTests
{
    public class VttTranscriptParserTests
    {
        [Fact]
        public void ParseTranscript_ReturnProperVtt_WithExtension()
        {
            // Arrange
            var rawData = File.ReadAllText("Data/transcript-result.json");
            var tresult = JsonConvert.DeserializeObject<TranscriptionResult>(rawData);
            var parser = new VttTranscriptParser();

            // Act
            var result = parser.Parse(tresult);

            // Asses
            Assert.Equal(".vtt", result.extension);
            Assert.Equal("WEBVTT\r\n\r\n00:00:03.920 --> 00:00:08.050\r\nTest.\r\n\r\n00:00:09.340 --> 00:00:13.790\r\nTest.\r\n\r\n", result.text);
        }
    }
}
