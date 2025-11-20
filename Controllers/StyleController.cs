using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebArchiver.Controllers
{
    [Route("api/styles")]
    [ApiController]
    public class StyleController : ControllerBase
    {
        private readonly IPageService _pageService;

        public StyleController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet("{id}")]
        public async Task<ContentResult> GetStyleById(string id)
        {
            var result = await _pageService.GetStyleById(id);
            if (string.IsNullOrEmpty(result))
                return new ContentResult { StatusCode = 404 };
            return new ContentResult { Content = result, ContentType = "text/css" };
        }
    }
}
