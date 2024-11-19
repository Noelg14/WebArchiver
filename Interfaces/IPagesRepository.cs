using WebArchiver.Entities;

namespace WebArchiver.Interfaces
{
    public interface IPagesRepository 
    {
        public Task<Pages> GetPageByIDAsync(string id);
        public Task<Pages> GetPageByUrlAsync(string url);
        public Task AddPageAsync(Pages page);
    }
}
