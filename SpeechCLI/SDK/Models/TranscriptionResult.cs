using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CRIS.Models
{
    public class TranscriptionResult
    {
        public AudioFileResult[] AudioFileResults { get; set; }
    }

    public class AudioFileResult
    {
        public string AudioFileName { get; set; }
        public SegmentResult[] SegmentResults { get; set; }
    }

    public class SegmentResult
    {
        public string RecognitionStatus { get; set; }
        public long Offset { get; set; }
        public int Duration { get; set; }
        public Nbest[] NBest { get; set; }
    }

    public class Nbest
    {
        public float Confidence { get; set; }
        public string Lexical { get; set; }
        public string ITN { get; set; }
        public string MaskedITN { get; set; }
        public string Display { get; set; }
        public Word[] Words { get; set; }
    }

    public class Word
    {
        [JsonProperty("Word")]
        public string WordText { get; set; }
        public long Offset { get; set; }
        public long Duration { get; set; }
    }

}
