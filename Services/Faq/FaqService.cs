using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Faqs;

namespace Shapper.Services.Faqs
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _faqRepository;
        private readonly IMapper _mapper;

        public FaqService(IFaqRepository faqRepository, IMapper mapper)
        {
            _faqRepository = faqRepository;
            _mapper = mapper;
        }

        public async Task<FaqResponseDto> CreateAsync(FaqDto dto)
        {
            var category = _mapper.Map<Faq>(dto);

            await _faqRepository.AddAsync(category);

            return _mapper.Map<FaqResponseDto>(category);
        }

        public async Task<FaqDto?> GetByIdAsync(int id)
        {
            var category = await _faqRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<FaqDto>(category);
        }

        public async Task<PagedResponseDto<FaqResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (Faqs, totalCount) = await _faqRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<FaqResponseDto>>(Faqs);

            return new PagedResponseDto<FaqResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<FaqResponseDto> UpdateAsync(int id, FaqDto dto)
        {
            var existingOrder = await _faqRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("Faq not found.");
            _mapper.Map(dto, existingOrder);

            await _faqRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<FaqResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _faqRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("Faq not found.");

            await _faqRepository.DeleteAsync(category);
        }
    }
}
