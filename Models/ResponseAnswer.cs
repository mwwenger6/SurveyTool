using Microsoft.EntityFrameworkCore;

namespace SurveyTool.Models
{
    [PrimaryKey(nameof(ResponseAnswerId))]
    public class TblResponseAnswer
    {
        public int ResponseAnswerId { get; set; }
        public int ResponseId { get; set; }
        public int QuestionId { get; set; }
        public byte? AnswerNumber { get; set; }
        public string? FreeText { get; set; }
    }
}