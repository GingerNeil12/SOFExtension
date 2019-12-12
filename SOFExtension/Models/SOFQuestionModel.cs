using Newtonsoft.Json;
using System.Collections.Generic;

namespace SOFExtension.Models
{
    public class SOFQuestionModel
    {
        public List<Item> Items { get; set; }

        public class Item
        {
            [JsonProperty("question_id")]
            public long QuestionId { get; set; }

            [JsonProperty("is_answered")]
            public bool IsAnswered { get; set; }

            [JsonProperty("answer_count")]
            public int AnswerCount { get; set; }

            [JsonProperty("body_markdown")]
            public string BodyMarkdown { get; set; }

            [JsonProperty("view_count")]
            public int ViewCount { get; set; }

            public string Link { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }
    }
}
