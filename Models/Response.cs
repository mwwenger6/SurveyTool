using Microsoft.EntityFrameworkCore;

namespace SurveyTool.Models
{
    public class Response : TblResponse
    {
        public List<TblResponseAnswer> ResponseAnswers { get; set; } = [];
    }
    [PrimaryKey(nameof(ResponseId))]
    public class TblResponse
    {
#nullable disable
        public int ResponseId { get; set; }
        public int SurveyId { get; set; } //FK to TblSurvey.SurveyId
        public DateTime ResponseSubmittedDate { get; set; }

    }
}
