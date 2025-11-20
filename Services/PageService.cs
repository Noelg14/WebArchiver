using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebArchiver.DTO.Response;
using WebArchiver.Entities;
using WebArchiver.Interfaces;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace WebArchiver.Services
{
    public class PageService : IPageService
    {
        private readonly ILogger<PageService> _logger;
        private HttpClient _httpClient;
        private IPagesRepository _pagesRepository;
        private IStylesRepository _styleRepository;
        private readonly string _apostrapheRegex =
            "(?<![\\[\\]()}{:;, } |?=])['](?![\\[\\]:;,()}{}?= |])";
        private string userAgent = string.Empty;

        public PageService(

            ILogger<PageService> logger,
            IPagesRepository pagesRepository,
            IStylesRepository styleRepository,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _pagesRepository = pagesRepository;
            _styleRepository = styleRepository;
            userAgent = configuration["userAgent"] ?? "WebArchiver/1.0 (+https://webarchiver20241121101430.azurewebsites.net)";
            initHttpClient();
        }

        private void initHttpClient()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(
                userAgent
            );
        }

        public async Task<string> GetPageAsync(string id)
        {
            _logger.LogInformation($"Getting page by Id {id}");
            var page = await _pagesRepository.GetPageByIDAsync(id);
            if (page is null)
                return string.Empty;

            return page.Content;
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
            //var title = doc.Title;
            var finalHtml = doc.ToHtml();
            finalHtml = Regex.Replace(finalHtml, _apostrapheRegex, "&#39;");

            var styles = doc.QuerySelectorAll("link").ToList();
            foreach (var style in styles)
            {
                if (style.GetAttribute("rel").Equals("stylesheet"))
                {
                    var styleUrl = style.GetAttribute("href");
                    var resp = parseStyleUrl(styleUrl, url);

                    _logger.LogInformation($"[INFO] StyleURL - {resp}");

                    var styleId = await saveStyleAsync(resp);
                    if (!string.IsNullOrEmpty(styleId))
                    {
                        _logger.LogInformation($"New Style ID : {styleId}");
                        finalHtml = finalHtml.Replace(styleUrl, $"/api/styles/{styleId}");
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"[INFO] Could not get stylesheet, using default..."
                        );
                        if (styleUrl.StartsWith("http"))
                            finalHtml = finalHtml.Replace(styleUrl, styleUrl);
                        else if (!url.EndsWith('/') && !styleUrl.StartsWith('/'))
                            finalHtml = finalHtml.Replace(styleUrl, url + "/" + styleUrl);
                        else
                            finalHtml = finalHtml.Replace(styleUrl, url + styleUrl);
                    }
                }
            }
            var page = new Pages
            {
                Id = RandomString(),
                URL = url,
                Content = finalHtml,
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
            return new string(
                Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }

        public async Task DeletePageById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");
            _logger.LogInformation($"Removing page with id : {id}");

            await _pagesRepository.DeletePage(id);
            return;
        }

        private string parseStyleUrl(string styleUrl, string url)
        {
            if (styleUrl.StartsWith('.'))
            {
                var arr = url.Split('/');
                arr[^1] = string.Empty; // remove last element
                url = string.Join("/", arr);
                styleUrl = styleUrl.Replace("./", "");
            }

            var root = "";
            if (!url.StartsWith("https://"))
                root = "http://" + url.Split("http://")[1].Split('/')[0];
            else
                root = "https://" + url.Split("https://")[1].Split('/')[0];

            _logger.LogInformation($"{root}");
            if (url.Contains("irishtimes"))
            {
                url = "https://irishtimes.com/";
            }
            if (!url.EndsWith('/'))
                url += "/";
            //if (url.EndsWith('/') && styleUrl.StartsWith('/'))
            //    styleUrl = styleUrl.Substring(1);
            if (!styleUrl.StartsWith("http") && styleUrl.StartsWith("//"))
                return "https:" + styleUrl;
            if (!styleUrl.StartsWith("http") && !styleUrl.StartsWith("//"))
                return url + styleUrl;
            return styleUrl;
        }

        public async Task<string> GetStyleById(string id)
        {
            var res = await _styleRepository.GetStyleByIdAsync(id);
            if (res is null)
                return string.Empty;
            return res;
        }

        private async Task<string> saveStyleAsync(string styleUrl)
        {
            try
            {
                var content = await _httpClient.GetStringAsync(styleUrl);
                if (string.IsNullOrEmpty(content))
                    return string.Empty;

                var style = new Styles { Content = content, Id = RandomString() };
                await _styleRepository.AddStyleAsync(style);
                return style.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Error][Page Service] Error Saving StyleSheet: {0}", ex.Message);
                return null;
            }
        }

        public async Task<ResponseDTO<PageResponseDTO>> GetAllPages(int Size, int Offset)
        {
            return await _pagesRepository.GetAllPages(Size, Offset);
        }
    }
}
