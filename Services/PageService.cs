using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebArchiver.Entities;
using WebArchiver.Interfaces;
namespace WebArchiver.Services
{
    public class PageService : IPageService
    {
        private readonly ILogger<PageService> _logger;
        private HttpClient _httpClient;
        private IPagesRepository _pagesRepository;
        public PageService(ILogger<PageService> logger,IPagesRepository pagesRepository)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _pagesRepository = pagesRepository;

            initHttpClient();
        }

        private void initHttpClient()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Googlebot");
        }

        public async Task<string> GetPageAsync(string id)
        {
            _logger.LogInformation($"Getting page by Id {id}");
            var page = await _pagesRepository.GetPageByIDAsync(id);

            return page.Content ?? string.Empty;
        }        
        public async Task<string> PostPageAsync(string url)
        {
            _logger.LogInformation($"Got Request with url {url}");
            if (string.IsNullOrEmpty(url))
                return string.Empty;


            var pageId = await GetPageByUrl(url);
            if (!string.IsNullOrEmpty(pageId))
            {

                _logger.LogInformation($"URL Exists returning - {pageId}");
                return pageId;

            }

            _logger.LogInformation($"URL does not exist archiving new page");

            var httpResp = await _httpClient.GetStringAsync(url);
            HtmlParser htmlParser = new HtmlParser();
            var doc = htmlParser.ParseDocument(httpResp);

            var page = new Pages
            {
                Id = RandomString(),
                URl = url,
                Content = doc.ToHtml()
            };
            await _pagesRepository.AddPageAsync(page);
            _logger.LogInformation($"Page added to DB - {page.Id}");
            return page.Id;
        }

        public async Task<string> GetPageByUrl(string url)
        {
            _logger.LogInformation($"Checking if page already exists : {url}");
            var page = await _pagesRepository.GetPageByUrlAsync(url);
            if (page is null)
                return string.Empty;
            else
                return page.Id;
        }
        private static Random random = new Random();

        private string RandomString()
        {
            int length = 10;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
