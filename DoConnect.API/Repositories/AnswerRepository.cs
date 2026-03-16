using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoConnect.API.Data;
using DoConnect.API.Models;

namespace DoConnect.API.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Answer> CreateAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.Answers
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.AnswerId == id);
        }

        public async Task<List<Answer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId && a.Status == "Approved")
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Answer>> GetByUserIdAsync(int userId)
        {
            return await _context.Answers
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Question)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Answer>> GetPendingAsync()
        {
            return await _context.Answers
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Question)
                .Where(a => a.Status == "Pending")
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Answer>> GetHistoryAsync()
        {
            return await _context.Answers
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Question)
                .Where(a => a.Status == "Approved" ||
                            a.Status == "Rejected" ||
                            a.Status == "Deleted")
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Answer answer)
        {
            answer.Status = "Deleted";

            _context.Answers.Update(answer);

            await _context.SaveChangesAsync();
        }
    }
}