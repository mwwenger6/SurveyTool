using Microsoft.EntityFrameworkCore;

namespace SurveyTool.Models
{
#nullable disable
    public class Question : TblQuestion
    {
        public List<TblAnswer> Answers { get; set; } = [];
        public string TypeName {
            get
            {
                if (QuestionTypeConstants.TypeDictionary.TryGetValue(TypeId, out string typeName))
                    return typeName;

                return "Unknown Type";
            }
        }
    }

    [PrimaryKey(nameof(QuestionId))]
    public class TblQuestion
    {
        public int QuestionId { get; set; }
        public byte TypeId { get; set; }
        public string Text { get; set; }
        public int SurveyId { get; set; }

    }
}
