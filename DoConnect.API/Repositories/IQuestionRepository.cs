using System.Collections.Generic;
using System.Threading.Tasks;
using DoConnect.API.Models;

namespace DoConnect.API.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> CreateAsync(Question question);

        Task<Question?> GetByIdAsync(int id);

        Task<List<Question>> GetAllApprovedAsync(int page, int pageSize);

        Task<List<Question>> SearchAsync(string query, int page, int pageSize);

        Task<List<Question>> GetPendingAsync();

        Task<List<Question>> GetHistoryAsync();

        Task<List<Question>> GetByUserIdAsync(int userId);

        Task<List<Question>> GetAllAsync(int page, int pageSize);

        Task UpdateAsync(Question question);

        Task DeleteAsync(Question question);

        Task<int> CountApprovedAsync();

        Task<int> CountSearchAsync(string query);
    }
}