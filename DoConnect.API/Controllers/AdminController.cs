using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoConnect.API.Repositories;
using DoConnect.API.Services;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;

        public AdminController(
            IQuestionRepository questionRepo,
            IAnswerRepository answerRepo,
            IQuestionService questionService,
            IAnswerService answerService)
        {
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _questionService = questionService;
            _answerService = answerService;
        }

        [HttpGet("pending-questions")]
        public async Task<IActionResult> GetPendingQuestions()
        {
            var questions = await _questionService.GetPendingAsync();
            return Ok(questions);
        }

        [HttpGet("pending-answers")]
        public async Task<IActionResult> GetPendingAnswers()
        {
            var answers = await _answerService.GetPendingAsync();
            return Ok(answers);
        }

        [HttpGet("history-questions")]
        public async Task<IActionResult> GetHistoryQuestions()
        {
            var questions = await _questionService.GetHistoryAsync();
            return Ok(questions);
        }

        [HttpGet("history-answers")]
        public async Task<IActionResult> GetHistoryAnswers()
        {
            var answers = await _answerService.GetHistoryAsync();
            return Ok(answers);
        }

        [HttpPut("approve-question/{id:int}")]
        public async Task<IActionResult> ApproveQuestion(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null) return NotFound(new { error = "Question not found." });
            question.Status = "Approved";
            await _questionRepo.UpdateAsync(question);
            return Ok(new { message = "Question approved." });
        }

        [HttpPut("reject-question/{id:int}")]
        public async Task<IActionResult> RejectQuestion(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null) return NotFound(new { error = "Question not found." });
            question.Status = "Rejected";
            await _questionRepo.UpdateAsync(question);
            return Ok(new { message = "Question rejected." });
        }

        [HttpDelete("delete-question/{id:int}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null) return NotFound(new { error = "Question not found." });
            await _questionRepo.DeleteAsync(question);
            return Ok(new { message = "Question deleted." });
        }

        [HttpPut("approve-answer/{id:int}")]
        public async Task<IActionResult> ApproveAnswer(int id)
        {
            var answer = await _answerRepo.GetByIdAsync(id);
            if (answer == null) return NotFound(new { error = "Answer not found." });
            answer.Status = "Approved";
            await _answerRepo.UpdateAsync(answer);
            return Ok(new { message = "Answer approved." });
        }

        [HttpPut("reject-answer/{id:int}")]
        public async Task<IActionResult> RejectAnswer(int id)
        {
            var answer = await _answerRepo.GetByIdAsync(id);
            if (answer == null) return NotFound(new { error = "Answer not found." });
            answer.Status = "Rejected";
            await _answerRepo.UpdateAsync(answer);
            return Ok(new { message = "Answer rejected." });
        }

        [HttpDelete("delete-answer/{id:int}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var answer = await _answerRepo.GetByIdAsync(id);
            if (answer == null) return NotFound(new { error = "Answer not found." });
            await _answerRepo.DeleteAsync(answer);
            return Ok(new { message = "Answer deleted." });
        }
    }
}
