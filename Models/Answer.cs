using Microsoft.EntityFrameworkCore;

namespace SurveyTool.Models
{
    //Clustered Key of AnswerNumber and QuestionId
    [PrimaryKey(nameof(AnswerNumber), nameof(QuestionId))]
    public class TblAnswer
    {
#nullable disable
        public byte AnswerNumber { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int Weight { get; set; }
    }
}
