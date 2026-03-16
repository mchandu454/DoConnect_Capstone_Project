using System.Collections.Generic;
using System.Threading.Tasks;
using DoConnect.API.Models;

namespace DoConnect.API.Repositories
{
    public interface IAnswerRepository
    {
        Task<Answer> CreateAsync(Answer answer);

        Task<Answer?> GetByIdAsync(int id);

        Task<List<Answer>> GetByQuestionIdAsync(int questionId);

        Task<List<Answer>> GetByUserIdAsync(int userId);

        Task<List<Answer>> GetPendingAsync();

        Task<List<Answer>> GetHistoryAsync();

        Task UpdateAsync(Answer answer);

        Task DeleteAsync(Answer answer);
    }
}