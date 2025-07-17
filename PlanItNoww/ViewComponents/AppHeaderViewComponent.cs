using Microsoft.AspNetCore.Mvc;
using PlanItNoww.Services;
using PlanItNoww.Utils;
using PlanItNoww.ViewModels;
using PlanItNoww.Models;
using Amazon.Util.Internal.PlatformServices;


namespace PlanItNoww.ViewComponents
{
    public class AppHeaderViewComponent : ViewComponent
    {
        private RequestState _requestState;
        
        public AppHeaderViewComponent(RequestState requestState)
        {
            _requestState = requestState;
           
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AppHeaderViewModel data = new AppHeaderViewModel();
           
            return View(data);
        }
    }
}
