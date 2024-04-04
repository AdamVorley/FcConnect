using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FcConnect.Areas.Identity.Pages.Account
{
    public class SuccessModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SuccessModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string SvgContent { get; private set; }

        public void OnGet()
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "success_.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }
    }
}
