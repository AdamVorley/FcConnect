using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FcConnect.Pages.Error
{
    [AllowAnonymous]
    public class SuspendedModel : PageModel

    {
        IWebHostEnvironment _webHostEnvironment;

        public SuspendedModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string SvgContent { get; private set; }

        public void OnGet()
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "suspended.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }
    }
}
