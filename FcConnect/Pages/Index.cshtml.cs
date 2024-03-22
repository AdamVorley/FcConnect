using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FcConnect.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;

        }

        public string SvgContent { get; private set; }


        public void OnGet()
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "thinking.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

        }
    }
}
