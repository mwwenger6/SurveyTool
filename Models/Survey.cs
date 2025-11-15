using Microsoft.EntityFrameworkCore;

namespace SurveyTool.Models
{
    public class Survey : TblSurvey
    {
        public List<Question> Questions { get; set; } = [];
    }

    [PrimaryKey(nameof(SurveyId))]
    public class TblSurvey
    {
#nullable disable
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }
}
