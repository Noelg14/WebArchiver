using Microsoft.EntityFrameworkCore;
using WebArchiver.Entities;
using WebArchiver.Interfaces;

namespace WebArchiver.Data.Repository
{
    public class StylesRepository : IStylesRepository
    {
        private PagesContext _context;

        public StylesRepository(PagesContext context)
        {
            _context = context;
        }

        public async Task AddStyleAsync(Styles style)
        {
            await _context.Styles.AddAsync(style);
            _context.SaveChanges();
            return;
        }

        public async Task<string> GetStyleByIdAsync(string id)
        {
            var res = _context.Styles.FirstOrDefault(s => s.Id == id);
            if (res is null)
            {
                return string.Empty;
            }
            return res.Content;
        }
    }
}
