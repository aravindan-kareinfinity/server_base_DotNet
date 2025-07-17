using Microsoft.AspNetCore.Mvc;
using PlanItNoww.Models;
using PlanItNoww.Services;
using PlanItNoww.Utils;
using PlanItNoww.ViewModels;

namespace PlanItNoww.ViewComponents
{
    public class AppFooterViewComponent : ViewComponent
    {

        RequestState requeststate;
        public AppFooterViewComponent( RequestState requeststate)
        {
            this.requeststate = requeststate;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppFooterViewModel data = new AppFooterViewModel();
            
            return View(data);
        }
    }
}
