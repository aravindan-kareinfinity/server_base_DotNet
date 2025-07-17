using Microsoft.AspNetCore.Mvc;
using PlanItNoww.Models;
using PlanItNoww.ViewModels;
using PlanItNoww.Services;

using System.Diagnostics;
using PlanItNoww.Utils;

namespace PlanItNoww.Controllers
{
    [Route("mobile")]
    public class HomeMobileController : Controller
    {
        private readonly ILogger<HomeMobileController> _logger;
        
        public HomeMobileController(ILogger<HomeMobileController> logger)
        {
            _logger = logger;
           
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View();


        }


        

    }
}