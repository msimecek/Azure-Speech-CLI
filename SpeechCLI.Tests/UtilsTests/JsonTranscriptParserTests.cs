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
    public class JsonTranscriptParserTests
    {
        [Fact]
        public void ParseTranscript_ReturnFullJson_WithExtension()
        {
            // Arrange
            var rawData = File.ReadAllText("Data/transcript-result.json");
            var tresult = JsonConvert.DeserializeObject<TranscriptionResult>(rawData);
            var parser = new JsonTranscriptParser();

            // Act
            var result = parser.Parse(tresult);

            // Asses
            Assert.Equal(".json", result.extension);
            Assert.Equal(rawData, result.text);
        }
    }
}
