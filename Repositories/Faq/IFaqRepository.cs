using Shapper.Dtos;
using Shapper.Dtos.Faqs;
using Shapper.Models;

namespace Shapper.Repositories.Faqs
{
    public interface IFaqRepository
    {
        Task<Faq> GetByIdAsync(int id);
        Task<(List<Faq> Faqs, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task AddAsync(Faq faq);
        Task<Faq> UpdateAsync(Faq faq);
        Task DeleteAsync(Faq faq);
    }
}
