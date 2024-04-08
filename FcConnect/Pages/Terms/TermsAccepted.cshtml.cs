using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FcConnect.Pages.Terms
{
    public class TermsAcceptedModel : PageModel
    {
        IWebHostEnvironment _webHostEnvironment;

        public TermsAcceptedModel(IWebHostEnvironment webHostEnvironment)
        {           
            _webHostEnvironment = webHostEnvironment;
        }
        public string SvgContent { get; private set; }

        public void OnGet()
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "terms_accepted.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }
    }
}
