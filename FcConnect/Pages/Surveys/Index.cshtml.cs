using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;
using Microsoft.AspNetCore.Authorization;

namespace FcConnect.Pages.Surveys
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IList<Survey> Survey { get;set; } = default!;
        public string SvgHeaderContent { get; private set; }
        public string SvgContent { get; private set; }
        public string CurrentFilter { get; set; }



        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                Survey = await _context.Survey.Where(s => s.Name.Contains(searchString)).ToListAsync();

            }
            else 
            {
                Survey = await _context.Survey.ToListAsync();
            }

            var svgHeaderFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "undraw_job_hunt_re_q203.svg");
            SvgHeaderContent = System.IO.File.ReadAllText(svgHeaderFilePath);

            if (Survey.Count == 0) 
            {
                var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "no_results.svg");
                SvgContent = System.IO.File.ReadAllText(svgFilePath);
            }
        }
    }
}
