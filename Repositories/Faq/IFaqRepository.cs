using Shapper.Models;

namespace Shapper.Repositories.Faqs
{
    public interface IFaqRepository
    {
        Task<Faq?> GetByIdAsync(int id);
        Task<List<Faq>> GetByStoreIdAsync(int storeId);
        Task<(List<Faq> Faqs, int TotalCount)> GetPaginatedAsync(int page, int pageSize, int? storeId = null);
        Task<bool> ExistsByStoreAndQuestionAsync(int storeId, string question, int? excludeId = null);
        Task AddAsync(Faq faq);
        Task<Faq> UpdateAsync(Faq faq);
        Task DeleteAsync(Faq faq);
    }
}