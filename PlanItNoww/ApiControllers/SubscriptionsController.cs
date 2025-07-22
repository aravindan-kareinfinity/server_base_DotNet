using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        ILogger<SubscriptionsController> logger;
        SubscriptionsService subscriptionsService;
        public SubscriptionsController(ILogger<SubscriptionsController> logger, SubscriptionsService subscriptionsService)
        {
            logger = logger;
            this.subscriptionsService = subscriptionsService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<Subscriptions>>> Entity()
        {
            ActionRes<Subscriptions> result = new ActionRes<Subscriptions>()
            {
               item = new Subscriptions()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<Subscriptions>>>> Select(ActionReq<SubscriptionsSelectReq> req)
        {
            ActionRes<List<Subscriptions>> result = new ActionRes<List<Subscriptions>>();

            result.item = await subscriptionsService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<Subscriptions>>> Insert(ActionReq<Subscriptions> req)
        {
            ActionRes<Subscriptions> result = new ActionRes<Subscriptions>();

            result.item = await subscriptionsService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<Subscriptions>>> Update(ActionReq<Subscriptions> req)
        {
            ActionRes<Subscriptions> result = new ActionRes<Subscriptions>();

            result.item = await subscriptionsService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<Subscriptions>>> Save(ActionReq<Subscriptions> req)
        {
            ActionRes<Subscriptions> result = new ActionRes<Subscriptions>();

            if(req.item.id > 0){
                result.item = await subscriptionsService.Update(req.item);
            }else{
                result.item = await subscriptionsService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<SubscriptionsDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await subscriptionsService.Delete(req.item);

            return Ok(result);
        }
    }
}
