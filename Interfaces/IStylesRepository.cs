using WebArchiver.Entities;

namespace WebArchiver.Interfaces
{
    public interface IStylesRepository
    {
        public Task<string> GetStyleByIdAsync(string id);
        public Task AddStyleAsync(Styles style);
    }
}
