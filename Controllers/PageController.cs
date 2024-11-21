using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebArchiver.DTO.Request;
using static System.Net.WebRequestMethods;

namespace WebArchiver.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;
        private readonly IConfiguration _configuration;
        public PageController(IPageService pageService,IConfiguration configuration)
        {
            _pageService = pageService; 
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetPage([FromQuery]string id)
        {
            var response = await _pageService.GetPageAsync(id);
            if(String.IsNullOrEmpty(response))
                return NotFound();
            return new ContentResult { 
                Content = response,
                ContentType = "text/html"
            };
        }        
        [HttpPost]
        public async Task<ActionResult> PostPage([FromBody]PageRequestDTO pageRequest)
        {
            var url = Request.Scheme+"/"+Request.Host;
            if (pageRequest is null || String.IsNullOrEmpty(pageRequest.URL))
                return BadRequest("URL is empty");

            var response = await _pageService.PostPageAsync(pageRequest.URL);

            var resUrl = _configuration["host"] + $"api/pages?id={response}";

            return RedirectPermanent(resUrl);
                
        }
        [HttpDelete]
        public async Task<ActionResult> DeletePage([FromQuery] string id)
        { 
            await _pageService.DeletePageById(id);

            return Ok();

        }
    }
}
