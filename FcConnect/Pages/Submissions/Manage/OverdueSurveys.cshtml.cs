﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FcConnect.Data;
using FcConnect.Models;

namespace FcConnect.Pages.Submissions.Manage
{
    public class OverdueSurveysModel : PageModel
    {
        private readonly FcConnect.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OverdueSurveysModel(FcConnect.Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IList<SurveyUserLink> SurveyUserLink { get;set; } = default!;
        public string SvgContent { get; set; }
        public string SvgContentNoResults { get; set; }

        public async Task OnGetAsync()
        {
            SurveyUserLink = await _context.SurveyUserLink.Include(s => s.User).Where(s => s.StatusId == Constants.StatusSurveyOutstanding && s.DateDue.Date < DateTime.Now.Date).OrderBy(s => s.DateDue).ToListAsync();

            var svgFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "late.svg");
            SvgContent = System.IO.File.ReadAllText(svgFilePath);

            if (SurveyUserLink.Count == 0) 
            {
                var svgNoResultsFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Assets", "caught_up.svg");
                SvgContentNoResults = System.IO.File.ReadAllText(svgNoResultsFilePath);
            }
        }
    }
}