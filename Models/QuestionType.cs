namespace SurveyTool.Models
{
    public class QuestionType
    {
#nullable disable
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class QuestionTypeConstants
    {
        public const string SINGLE_CHOICE = "Single Choice";
        public const string MULTIPLE_CHOICE = "Multiple Choice";
        public const string FREE_TEXT = "Free Text";

        public static readonly IReadOnlyDictionary<int, string> TypeDictionary = new Dictionary<int, string>
        {
            { 1, SINGLE_CHOICE },
            { 2, MULTIPLE_CHOICE },
            { 3, FREE_TEXT },
        };
    }
}
