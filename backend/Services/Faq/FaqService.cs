using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Faqs;
using Shapper.Models;
using Shapper.Repositories.Faqs;
using Shapper.Repositories.Stores;

namespace Shapper.Services.Faqs
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _faqRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public FaqService(
            IFaqRepository faqRepository,
            IStoreRepository storeRepository,
            IMapper mapper)
        {
            _faqRepository = faqRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<FaqResponseDto> CreateAsync(FaqDto dto)
        {
            var store = await _storeRepository.GetByIdAsync(dto.StoreId);
            if (store == null)
                throw new InvalidOperationException($"Store with ID {dto.StoreId} does not exist.");

            var exists = await _faqRepository.ExistsByStoreAndQuestionAsync(dto.StoreId, dto.Question);
            if (exists)
                throw new InvalidOperationException("A FAQ with this question already exists for this store.");

            var faq = _mapper.Map<Faq>(dto);
            faq.CreatedAt = DateTime.UtcNow;

            await _faqRepository.AddAsync(faq);

            return await MapToResponseAsync(faq);
        }

        public async Task<FaqResponseDto?> GetByIdAsync(int id)
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            return faq == null ? null : await MapToResponseAsync(faq);
        }

        public async Task<List<FaqResponseDto>> GetByStoreIdAsync(int storeId)
        {
            var faqs = await _faqRepository.GetByStoreIdAsync(storeId);
            var responses = new List<FaqResponseDto>();

            foreach (var faq in faqs)
            {
                responses.Add(await MapToResponseAsync(faq));
            }

            return responses;
        }

        public async Task<List<FaqResponseDto>> GetByStoreCodeAsync(string storeCode)
        {
            var faqs = await _faqRepository.GetByStoreCodeAsync(storeCode);
            var responses = new List<FaqResponseDto>();

            foreach (var faq in faqs)
            {
                responses.Add(await MapToResponseAsync(faq));
            }

            return responses;
        }

        public async Task<PagedResponseDto<FaqResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeId = null)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (faqs, totalCount) = await _faqRepository.GetPaginatedAsync(page, pageSize, storeId);

            var mapped = new List<FaqResponseDto>();
            foreach (var faq in faqs)
            {
                mapped.Add(await MapToResponseAsync(faq));
            }

            return new PagedResponseDto<FaqResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<PagedResponseDto<FaqResponseDto>> GetPaginatedByStoreCodeAsync(
            int page,
            int pageSize,
            string? storeCode = null)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (faqs, totalCount) = await _faqRepository.GetPaginatedByStoreCodeAsync(page, pageSize, storeCode);

            var mapped = new List<FaqResponseDto>();
            foreach (var faq in faqs)
            {
                mapped.Add(await MapToResponseAsync(faq));
            }

            return new PagedResponseDto<FaqResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<FaqResponseDto> UpdateAsync(int id, FaqUpdateDto dto)
        {
            var existingFaq = await _faqRepository.GetByIdAsync(id);
            if (existingFaq == null)
                throw new InvalidOperationException("FAQ not found.");

            if (!string.IsNullOrWhiteSpace(dto.Question))
            {
                var exists = await _faqRepository.ExistsByStoreAndQuestionAsync(
                    existingFaq.StoreId,
                    dto.Question,
                    id);

                if (exists)
                    throw new InvalidOperationException("A FAQ with this question already exists for this store.");

                existingFaq.Question = dto.Question;
            }

            if (!string.IsNullOrWhiteSpace(dto.Answer))
                existingFaq.Answer = dto.Answer;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                existingFaq.Status = dto.Status;

            existingFaq.UpdatedAt = DateTime.UtcNow;

            await _faqRepository.UpdateAsync(existingFaq);

            return await MapToResponseAsync(existingFaq);
        }

        public async Task DeleteAsync(int id)
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            if (faq == null)
                throw new InvalidOperationException("FAQ not found.");

            await _faqRepository.DeleteAsync(faq);
        }

        public async Task<FaqResponseDto> ToggleStatusAsync(int id)
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            if (faq == null)
                throw new InvalidOperationException("FAQ not found.");

            faq.Status = faq.Status == "ACTIVE" ? "INACTIVE" : "ACTIVE";
            faq.UpdatedAt = DateTime.UtcNow;

            await _faqRepository.UpdateAsync(faq);

            return await MapToResponseAsync(faq);
        }

        private async Task<FaqResponseDto> MapToResponseAsync(Faq faq)
        {
            var response = _mapper.Map<FaqResponseDto>(faq);

            if (faq.Store != null)
            {
                response.StoreName = faq.Store.Name;
                response.StoreCode = faq.Store.StoreCode;
            }
            else if (faq.StoreId > 0)
            {
                var store = await _storeRepository.GetByIdAsync(faq.StoreId);
                response.StoreName = store?.Name ?? string.Empty;
                response.StoreCode = store?.StoreCode ?? string.Empty;
            }

            return response;
        }
    }
}