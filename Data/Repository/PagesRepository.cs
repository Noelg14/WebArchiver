using Microsoft.EntityFrameworkCore;
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
            var res = await _context.AddAsync(page);
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
    }
}
