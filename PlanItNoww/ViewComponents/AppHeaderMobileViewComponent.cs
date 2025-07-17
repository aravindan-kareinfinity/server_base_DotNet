using Microsoft.AspNetCore.Mvc;
using PlanItNoww.Models;
using PlanItNoww.Services;
using PlanItNoww.Utils;
using PlanItNoww.ViewModels;

namespace PlanItNoww.ViewComponents
{
    public class AppHeaderMobileViewComponent : ViewComponent
    {
        private RequestState _requestState;
        
        public AppHeaderMobileViewComponent(RequestState requestState)
        {
            _requestState = requestState;
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppHeaderMobileViewModel data = new AppHeaderMobileViewModel();
            
            return View(data);
        }
    }
}
