using Microsoft.AspNetCore.Mvc;
using PlanItNoww.Models;
using PlanItNoww.Services;
using PlanItNoww.Utils;
using PlanItNoww.ViewModels;

namespace PlanItNoww.ViewComponents
{
    public class AppFooterMobileViewComponent : ViewComponent
    {
        RequestState requeststate;
        public AppFooterMobileViewComponent(RequestState requeststate)
        {
            this.requeststate = requeststate;
           
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppFooterMobileViewModel data = new AppFooterMobileViewModel();
            

            return View(data);
        }
    }
}
