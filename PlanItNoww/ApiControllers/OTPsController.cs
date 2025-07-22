using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPsController : ControllerBase
    {
        ILogger<OTPsController> logger;
        OTPsService otpsService;
        public OTPsController(ILogger<OTPsController> logger, OTPsService otpsService)
        {
            logger = logger;
            this.otpsService = otpsService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<OTPs>>> Entity()
        {
            ActionRes<OTPs> result = new ActionRes<OTPs>()
            {
               item = new OTPs()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<OTPs>>>> Select(ActionReq<OTPsSelectReq> req)
        {
            ActionRes<List<OTPs>> result = new ActionRes<List<OTPs>>();

            result.item = await otpsService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<OTPs>>> Insert(ActionReq<OTPs> req)
        {
            ActionRes<OTPs> result = new ActionRes<OTPs>();

            result.item = await otpsService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<OTPs>>> Update(ActionReq<OTPs> req)
        {
            ActionRes<OTPs> result = new ActionRes<OTPs>();

            result.item = await otpsService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<OTPs>>> Save(ActionReq<OTPs> req)
        {
            ActionRes<OTPs> result = new ActionRes<OTPs>();

            if(req.item.id > 0){
                result.item = await otpsService.Update(req.item);
            }else{
                result.item = await otpsService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<OTPsDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await otpsService.Delete(req.item);

            return Ok(result);
        }
    }
}
