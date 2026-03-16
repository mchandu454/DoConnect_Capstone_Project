using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoConnect.API.DTOs;
using DoConnect.API.Services;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly IWebHostEnvironment _env;

        public AnswersController(IAnswerService answerService, IWebHostEnvironment env)
        {
            _answerService = answerService;
            _env = env;
        }

        [HttpGet("question/{questionId:int}")]
        public async Task<IActionResult> GetByQuestion(int questionId)
        {
            var answers = await _answerService.GetByQuestionIdAsync(questionId);
            return Ok(answers);
        }

        [HttpGet("my-answers")]
        [Authorize]
        public async Task<IActionResult> GetMyAnswers()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var items = await _answerService.GetByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateAnswerDto dto, IFormFile? image)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            string? imagePath = null;
            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);
                imagePath = $"/uploads/{fileName}";
            }
            var result = await _answerService.UpdateOwnAsync(id, userId, dto, imagePath);
            if (result == null) return NotFound(new { error = "Answer not found or you are not the owner." });
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var ok = await _answerService.DeleteOwnAsync(id, userId);
            if (!ok) return NotFound(new { error = "Answer not found or you are not the owner." });
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateAnswerDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            string? imagePath = null;
            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);
                imagePath = $"/uploads/{fileName}";
            }

            var result = await _answerService.CreateAsync(dto, userId, imagePath);
            if (result == null) return BadRequest(new { error = "Question not found or not approved." });

            return Ok(result);
        }
    }
}
