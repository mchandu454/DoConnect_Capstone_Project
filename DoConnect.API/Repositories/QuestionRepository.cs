using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoConnect.API.Data;
using DoConnect.API.Models;

namespace DoConnect.API.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(q => q.QuestionId == id);
        }

        public async Task<List<Question>> GetAllApprovedAsync(int page, int pageSize)
        {
            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Include(q => q.Answers)
                .Where(q => q.Status == "Approved")
                .OrderByDescending(q => q.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Question>> SearchAsync(string query, int page, int pageSize)
        {
            query = query.ToLower();

            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Include(q => q.Answers)
                .Where(q => q.Status == "Approved" &&
                       (q.Title.ToLower().Contains(query) ||
                        q.Description.ToLower().Contains(query)))
                .OrderByDescending(q => q.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Question>> GetPendingAsync()
        {
            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Where(q => q.Status == "Pending")
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Question>> GetHistoryAsync()
        {
            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Where(q => q.Status == "Approved" ||
                            q.Status == "Rejected" ||
                            q.Status == "Deleted")
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Question>> GetByUserIdAsync(int userId)
        {
            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Include(q => q.Answers)
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Question>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Questions
                .AsNoTracking()
                .Include(q => q.User)
                .Include(q => q.Images)
                .Include(q => q.Answers)
                .OrderByDescending(q => q.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Question question)
        {
            question.Status = "Deleted";
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountApprovedAsync()
        {
            return await _context.Questions
                .AsNoTracking()
                .CountAsync(q => q.Status == "Approved");
        }

        public async Task<int> CountSearchAsync(string query)
        {
            query = query.ToLower();

            return await _context.Questions
                .AsNoTracking()
                .CountAsync(q => q.Status == "Approved" &&
                       (q.Title.ToLower().Contains(query) ||
                        q.Description.ToLower().Contains(query)));
        }
    }
}