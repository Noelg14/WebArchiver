using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebArchiver.DTO.Response;
using WebArchiver.Entities;
using WebArchiver.Interfaces;

namespace WebArchiver.Data.Repository
{
    public class PagesRepository : IPagesRepository
    {
        private PagesContext _context;
        public PagesRepository(PagesContext context)
        {
            _context = context;
        }

        public async Task AddPageAsync(Pages page)
        {
            await _context.AddAsync(page);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task DeletePage(string id)
        {
            var entity = await GetPageByIDAsync(id);
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<Pages> GetPageByIDAsync(string id)
        {
            return await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pages> GetPageByUrlAsync(string url)
        {
            return await _context.Pages.FirstOrDefaultAsync(p => p.URl == url);
        }        
        public async Task<ResponseDTO<PageResponseDTO>> GetAllPages(int Size, int Offset)
        {
            var total = await _context.Pages.CountAsync();
            var query = _context.Pages.Select(p => new PageResponseDTO
            {
                Id = p.Id,
                Url = p.URl,
                Created = p.Created
            });

            if (Offset > 0)
                query = query.Skip(Offset);
            if (Size > 0)
                query = query.Take(Size);

            var data = await query.ToListAsync();
            var currPageIndex = (Size < data.Count ? Size : data.Count) + Offset;

            return new ResponseDTO<PageResponseDTO>
            {
                Data = data,
                TotalPages = data.Count,
                MoreRecords = currPageIndex < total ? true : false
            };
        }
    }
}
