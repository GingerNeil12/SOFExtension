using Newtonsoft.Json;
using System.Collections.Generic;

namespace SOFExtension.Models
{
    public class SOFAnswerModel
    {
        public List<Answer> Items { get; set; }

        public class Answer
        {
            [JsonProperty( "is_accepted" )]
            public bool IsAccepted { get; set; }

            [JsonProperty("body_markdown")]
            public string BodyMarkdown { get; set; }

            public int Score { get; set; }
            public string Body { get; set; }
        }
    }
}
