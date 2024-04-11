// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FcConnect.Areas.Identity.Pages.Account
{

    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        IWebHostEnvironment _webHostEnvironment;

        public ForgotPasswordConfirmation(IWebHostEnvironment webHostEnvironment) 
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string SvgContent { get; set; }

        public void OnGet()
        {
            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "pwd_reset_sent.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);
        }
    }
}
