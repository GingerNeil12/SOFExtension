using Newtonsoft.Json;
using System.Collections.Generic;

namespace SOFExtension.Models
{
    public class SOFSearchModel
    {
        public List<Item> Items { get; set; }

        public class Item
        {
            [JsonProperty("is_answered")]
            public bool IsAnswered { get; set; }

            [JsonProperty("view_count")]
            public int ViewCount { get; set; }

            [JsonProperty("answer_count")]
            public int AnswerCount { get; set; }

            [JsonProperty("question_id")]
            public long QuestionId { get; set; }

            public int Score { get; set; }
            public string Link { get; set; }
            public string Title { get; set; }
        }

    }
}
