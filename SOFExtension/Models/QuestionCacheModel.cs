using System;
using System.Collections.Generic;

namespace SOFExtension.Models
{
    public class QuestionCacheModel
    {
        public long QuestionId { get; set; }
        public SOFQuestionModel.Item Question { get; set; }
        public List<SOFAnswerModel> Answers { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
