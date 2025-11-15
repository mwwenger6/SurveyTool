using Microsoft.EntityFrameworkCore;
using SurveyTool.Models;

namespace SurveyTool.Services
{
    public class SurveyDataTools
    {
        private readonly SurveyDbContext SurveyDB;
        public SurveyDataTools(SurveyDbContext surveyDB) { SurveyDB = surveyDB; }


        #region Survey
        #region Getters
        public IQueryable<TblSurvey> GetTblSurveys() => SurveyDB.TblSurveys;
        public async Task<TblSurvey?> GetTblSurveyAsync (int id)
            => await GetTblSurveys().SingleOrDefaultAsync(s => s.SurveyId == id);
        #endregion
        #region Setters
        //Add Survey
        public async Task<int> AddTblSurveyAsync(Survey survey)
        {
            //await using var transaction = await SurveyDB.Database.BeginTransactionAsync();
            TblSurvey tblSurvey = new()
            {
                Description = survey.Description,
                Title = survey.Title,
            };

            await SurveyDB.TblSurveys.AddAsync(tblSurvey);
            await SurveyDB.SaveChangesAsync();

            if (survey.Questions.Count > 0)
            {
                foreach (Question question in survey.Questions)
                {
                    TblQuestion tblQuestion = new()
                    {
                        SurveyId = tblSurvey.SurveyId,
                        Text = question.Text,
                        TypeId = question.TypeId,
                    };
                    await SurveyDB.TblQuestions.AddAsync(tblQuestion);
                    await SurveyDB.SaveChangesAsync();

                    if (question.Answers.Count > 0)
                    {
                        for (int i = 0; i < question.Answers.Count; i++)
                        {
                            question.Answers[i].AnswerNumber = (byte)(i + 1);
                            question.Answers[i].QuestionId = tblQuestion.QuestionId;

                            await SurveyDB.TblAnswers.AddAsync(question.Answers[i]);
                            await SurveyDB.SaveChangesAsync();
                        }
                    }
                }
            }

            //await transaction.CommitAsync();

            return tblSurvey.SurveyId;
        }
        //Update Survey
        public async Task<bool> UpdateTblSurveyAsync(TblSurvey survey)
        {
            TblSurvey? existingSurvey = await SurveyDB.TblSurveys
                .FirstOrDefaultAsync(s => s.SurveyId == survey.SurveyId);

            if (existingSurvey == null)
                return false;

            existingSurvey.Description = survey.Description;
            existingSurvey.Title = survey.Title;

            int rowsAffected = await SurveyDB.SaveChangesAsync();

            return rowsAffected > 0;
        }

        //Delete Survey
        public async Task<bool> DeleteTblSurveyAsync(int id)
        {
            //await using var transaction = await SurveyDB.Database.BeginTransactionAsync();
            TblSurvey? survey = await GetTblSurveyAsync(id);
            if (survey == null) return false;

            List<int> questionIds = await SurveyDB.TblQuestions
                .Where(q => q.SurveyId == id)
                .Select(q => q.QuestionId).ToListAsync();

            if (questionIds.Count > 0)
            {
                List<TblAnswer> answers = await SurveyDB.TblAnswers
                .Where(a => questionIds.Contains(a.QuestionId))
                .ToListAsync();
                SurveyDB.TblAnswers.RemoveRange(answers);
            }

            List<TblQuestion> questions = await SurveyDB.TblQuestions
                .Where(q => q.SurveyId == id)
                .ToListAsync();
            SurveyDB.TblQuestions.RemoveRange(questions);
            SurveyDB.TblSurveys.Remove(survey);

            //await transaction.CommitAsync();
            return true;
        }
        #endregion
        #endregion
        #region Question
        #region Getters
        public async Task<TblQuestion?> GetTblQuestionAsync(int id)
            => await SurveyDB.TblQuestions.SingleOrDefaultAsync(q => q.QuestionId == id);

        public List<Question> GetQuestionsWithAnswersForSurvey(int surveyId)
        {
            List<TblQuestion> tblQuestions = SurveyDB.TblQuestions
                .Where(q => q.SurveyId == surveyId).ToList();

            List<Question> questionsWithAnswers = tblQuestions.Select(tblQ =>
            {
                Question question = new()
                {
                    QuestionId = tblQ.QuestionId,
                    TypeId = tblQ.TypeId,
                    Text = tblQ.Text,
                    SurveyId = tblQ.SurveyId,
                    Answers = GetTblAnswersForQuestion(tblQ.QuestionId).ToList()
                };
                return question;

            }).ToList();

            return questionsWithAnswers;
        }
        #endregion
        #region Setters
        //Add Question and Question answers (if applicable)
        public async Task<int> AddTblQuestionAsync(Question question)
        {
            //await using var transaction = await SurveyDB.Database.BeginTransactionAsync();

            TblQuestion tblQuestion = new()
            {
                SurveyId = question.SurveyId,
                Text = question.Text,
                TypeId = question.TypeId,
            };

            await SurveyDB.TblQuestions.AddAsync(tblQuestion);
            await SurveyDB.SaveChangesAsync();

            foreach (TblAnswer updatedAnswer in question.Answers)
            {
                updatedAnswer.QuestionId = tblQuestion.QuestionId;
            }

            await SurveyDB.TblAnswers.AddRangeAsync(question.Answers);
            await SurveyDB.SaveChangesAsync();

            //await transaction.CommitAsync();
            return tblQuestion.QuestionId;
        }

        //Update Question and answers (if applicable)
        public async Task<bool> UpdateTblQuestionAsync(Question question)
        {
            //await using var transaction = await SurveyDB.Database.BeginTransactionAsync();

            TblQuestion? existingQuestion = await SurveyDB.TblQuestions
                .FirstOrDefaultAsync(q => q.QuestionId == question.QuestionId);

            if (existingQuestion == null)
                return false;

            existingQuestion.Text = question.Text;
            existingQuestion.TypeId = question.TypeId;

            List<TblAnswer> existingAnswers = await SurveyDB.TblAnswers
                .Where(a => a.QuestionId == question.QuestionId)
                .ToListAsync();

            if (existingAnswers.Count > 0)
                SurveyDB.TblAnswers.RemoveRange(existingAnswers);

            foreach (TblAnswer newAnswer in question.Answers)
            {
                newAnswer.QuestionId = existingQuestion.QuestionId;
            }

            await SurveyDB.TblAnswers.AddRangeAsync(question.Answers);

            int rowsAffected = await SurveyDB.SaveChangesAsync();

            return rowsAffected > 0;
        }

        //Delete Question and answers (if applicable)
        public async Task<bool> DeleteTblQuestion(int id)
        {
            TblQuestion? question = await GetTblQuestionAsync(id);
            if (question == null)
                return false;

            List<TblAnswer> answersToDelete = await SurveyDB.TblAnswers
                .Where(a => a.QuestionId == id)
                .ToListAsync();

            if (answersToDelete.Count > 0)
                SurveyDB.TblAnswers.RemoveRange(answersToDelete);

            SurveyDB.TblQuestions.Remove(question);

            int rowsAffected = await SurveyDB.SaveChangesAsync();

            return rowsAffected > 0;
        }

        #endregion
        #endregion
        #region Answer
        #region Getters
        public IQueryable<TblAnswer> GetTblAnswersForQuestion(int questionId)
            => SurveyDB.TblAnswers.Where(a => a.QuestionId == questionId);
        #endregion
        #endregion
        #region Response
        #region Setters
        public async Task<int> AddResponseAsync(Response response)
        {
            //await using var transaction = await SurveyDB.Database.BeginTransactionAsync();

            TblResponse tblResponse = new()
            {
                ResponseSubmittedDate = DateTime.UtcNow,
                SurveyId = response.SurveyId,
            };

            await SurveyDB.TblResponses.AddAsync(tblResponse);
            await SurveyDB.SaveChangesAsync();

            foreach (TblResponseAnswer answer in response.ResponseAnswers)
            {
                answer.ResponseId = tblResponse.ResponseId;
            }

            await SurveyDB.TblResponseAnswers.AddRangeAsync(response.ResponseAnswers);
            await SurveyDB.SaveChangesAsync();

            //await transaction.CommitAsync();
            return tblResponse.ResponseId;
        }
        #endregion
        #endregion
        #region Scoring
        #region Getters
        public async Task<string?> CalculateResponseScoreAsync(int id)
        {
            TblResponse? response = await SurveyDB.TblResponses
                .SingleOrDefaultAsync(r => r.ResponseId == id);
            if (response == null) return null;

            List<TblResponseAnswer> responseAnswers = await SurveyDB.TblResponseAnswers
                .Where(a => a.ResponseId == id).ToListAsync();

            List<TblQuestion> surveyQuestions = await SurveyDB.TblQuestions
                .Where(a => a.SurveyId == response.SurveyId).ToListAsync();

            int achievedScore = 0;
            int maxScore = 0;

            foreach (TblResponseAnswer responseAnswer in responseAnswers)
            {
                if (responseAnswer.AnswerNumber != null)
                {
                    TblAnswer tblAnswer = await SurveyDB.TblAnswers
                        .SingleAsync(a => a.AnswerNumber == responseAnswer.AnswerNumber && a.QuestionId == responseAnswer.QuestionId);

                    achievedScore += tblAnswer.Weight;
                }
            }

            foreach (TblQuestion question in surveyQuestions)
            {
                List<TblAnswer> allQuestionAnswers = await SurveyDB.TblAnswers
                    .Where(a => a.QuestionId == question.QuestionId)
                    .ToListAsync();

                if (allQuestionAnswers.Count > 0)
                {
                    if (question.TypeId == 1)
                        maxScore += allQuestionAnswers.Max(a => a.Weight);
                    else if (question.TypeId == 2)
                        maxScore += allQuestionAnswers.Sum(a => a.Weight);
                }
            }

            return $"{achievedScore}/{maxScore}";
        }
        #endregion
        #endregion
    }
}
