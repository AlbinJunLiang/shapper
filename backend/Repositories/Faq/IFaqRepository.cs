using Shapper.Models;

namespace Shapper.Repositories.Faqs
{
    public interface IFaqRepository
    {
        // Get by ID
        Task<Faq?> GetByIdAsync(int id);

        // Get by Store (convenience methods)
        Task<List<Faq>> GetByStoreIdAsync(int storeId);
        Task<List<Faq>> GetByStoreCodeAsync(string storeCode);

        // Paginated (con StoreId - original)
        Task<(List<Faq> Faqs, int TotalCount)> GetPaginatedAsync(int page, int pageSize, int? storeId = null);

        // Paginated (con StoreCode - nuevo)
        Task<(List<Faq> Faqs, int TotalCount)> GetPaginatedByStoreCodeAsync(int page, int pageSize, string? storeCode = null);

        // Validations
        Task<bool> ExistsByStoreAndQuestionAsync(int storeId, string question, int? excludeId = null);
        Task<int> CountByStoreAsync(int storeId);
        Task<int> CountByStoreCodeAsync(string storeCode);

        // CRUD Operations
        Task AddAsync(Faq faq);
        Task<Faq> UpdateAsync(Faq faq);
        Task DeleteAsync(Faq faq);
    }
}