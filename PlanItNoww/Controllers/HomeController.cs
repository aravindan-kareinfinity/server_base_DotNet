     using Microsoft.AspNetCore.Mvc;
using PlanItNoww.Models;
using PlanItNoww.ViewModels;
using PlanItNoww.Services;

using System.Diagnostics;
using PlanItNoww.Utils;

namespace PlanItNoww.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        RequestState requeststate;
        public HomeController(ILogger<HomeController> logger, RequestState requeststate)
        {
            _logger = logger;
            
            this.requeststate = requeststate;
        }

        //public async Task<IActionResult> Index()
        //{
        //    return View();
        //}

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        

    }

}