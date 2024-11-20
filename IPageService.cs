namespace WebArchiver
{
    public interface IPageService
    {
        public Task<string> GetPageAsync(string id);
        public Task<string> PostPageAsync(string url);  
        public Task<string> GetPageByUrl(string url);  
        public Task DeletePageById(string id);  
        public Task<string> GetStyleById(string id);  
    }
}
