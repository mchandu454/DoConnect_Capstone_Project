using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoConnect.API.DTOs;
using DoConnect.API.Services;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IWebHostEnvironment _env;

        public QuestionsController(IQuestionService questionService, IWebHostEnvironment env)
        {
            _questionService = questionService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (items, total) = await _questionService.GetAllApprovedAsync(page, pageSize);
            return Ok(new { items, total, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound(new { error = "Question not found." });
            return Ok(question);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { error = "Search query is required." });
            var (items, total) = await _questionService.SearchAsync(q, page, pageSize);
            return Ok(new { items, total, page, pageSize, query = q });
        }

        [HttpGet("my-questions")]
        [Authorize]
        public async Task<IActionResult> GetMyQuestions()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var items = await _questionService.GetByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateQuestionDto dto, IFormFile? image)
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
            var result = await _questionService.UpdateOwnAsync(id, userId, dto, imagePath);
            if (result == null) return NotFound(new { error = "Question not found or you are not the owner." });
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var ok = await _questionService.DeleteOwnAsync(id, userId);
            if (!ok) return NotFound(new { error = "Question not found or you are not the owner." });
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateQuestionDto dto, IFormFile? image)
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

            var result = await _questionService.CreateAsync(dto, userId, imagePath);
            return CreatedAtAction(nameof(GetById), new { id = result.QuestionId }, result);
        }
    }
}
