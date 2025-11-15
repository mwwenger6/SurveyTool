using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyTool.Models;
using SurveyTool.Services;

namespace SurveyTool.Controllers
{
    [ApiController]
    [Route("Survey/[action]")]
    public class SurveyController : ControllerBase
    {
        private readonly SurveyDataTools _SurveyDataTools;

        public SurveyController(SurveyDataTools surveyTools) { _SurveyDataTools = surveyTools; }

        #region Survey
        //Add Survey
        [HttpPost]
        public async Task<IActionResult> AddSurvey([FromBody] Survey survey)
        {
            if (survey == null) return BadRequest("Survey data is missing.");

            try
            {
                await _SurveyDataTools.AddTblSurveyAsync(survey);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the survey.");
            }

            return Ok();
        }

        //Get Surveys
        [HttpGet]
        public IActionResult GetSurveys()
        {
            try
            {
                List<TblSurvey> surveys = _SurveyDataTools.GetTblSurveys().ToList();
                return Ok(surveys);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving surveys.");
            }
        }

        //Get Survey
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            try
            {
                TblSurvey? tblSurvey = await _SurveyDataTools.GetTblSurveyAsync(id);
                if (tblSurvey == null) return NotFound($"Survey with Id {id} was not found.");
                return Ok(tblSurvey);
            }
            catch (Exception) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving the survey.");
            }
        }

        //Update Survey
        [HttpPut]
        public async Task<IActionResult> UpdateSurvey([FromBody] TblSurvey survey)
        {
            if (survey == null)
                return BadRequest("Survey data is missing.");

            try
            {
                bool wasUpdated = await _SurveyDataTools.UpdateTblSurveyAsync(survey);
                if (!wasUpdated)
                    return NotFound($"Survey with Id {survey.SurveyId} was not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the survey.");
            }

            return NoContent();
        }


        //Delete Survey
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            try
            {
                bool deletedSurvey = await _SurveyDataTools.DeleteTblSurveyAsync(id);
                if (!deletedSurvey) return NotFound($"Survey with Id {id} was not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the survey.");
            }
            return NoContent();
        }
        #endregion

        #region Question
        //Add Question
        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            if (question == null) return BadRequest("Question data is required");
            try
            {
                TblSurvey? tblSurvey = await _SurveyDataTools.GetTblSurveyAsync(question.SurveyId);
                if (tblSurvey == null) return NotFound($"Survey with Id {question.SurveyId} was not found.");
                question.QuestionId = await _SurveyDataTools.AddTblQuestionAsync(question);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the question.");
            }

            return Ok();
        }

        //Get Questions From Survey
        [HttpGet("{surveyId:int}")]
        public IActionResult GetQuestionsForSurvey(int surveyId)
        {
            try
            {
                List<Question> questions = _SurveyDataTools.GetQuestionsWithAnswersForSurvey(surveyId).ToList();
                return Ok(questions);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving questions for survey.");
            }
        }


        //Update Survey
        [HttpPut]
        public async Task<IActionResult> UpdateQuestion([FromBody] Question question)
        {
            if (question == null)
                return BadRequest("Question is missing information");

            try
            {
                bool wasUpdated = await _SurveyDataTools.UpdateTblQuestionAsync(question);
                if (!wasUpdated)
                    return NotFound($"Question with ID {question.QuestionId} was not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the question.");
            }

            return Ok("Question and answers successfully replaced.");
        }

        //Delete Survey
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                bool availableQuestion = await _SurveyDataTools.DeleteTblQuestion(id);
                if (!availableQuestion) return NotFound($"Question with ID {id} was not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "An error occurred while deleting the question.");
            }
            return NoContent();
        }
        #endregion

        #region Response
        [HttpPost]
        public async Task<IActionResult> SubmitResponse([FromBody] Response response)
        {
            if (response == null || response.ResponseAnswers.Count == 0)
                return BadRequest("Response Data is missing.");
            try
            {
                response.ResponseId = await _SurveyDataTools.AddResponseAsync(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occured while submitting the response");
            }
            return Ok(response.ResponseId);
        }
        #endregion

        #region Scoring
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetResponseScore(int id)
        {
            try
            {
                string? score = await _SurveyDataTools.CalculateResponseScoreAsync(id);
                if (score == null) return NotFound($"Response with Id {id} was not found.");
                return Ok(score);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while calculating the score.");
            }
        }
        #endregion
    }
}
