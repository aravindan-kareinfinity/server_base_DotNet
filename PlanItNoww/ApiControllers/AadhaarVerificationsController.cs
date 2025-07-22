using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AadhaarVerificationsController : ControllerBase
    {
        ILogger<AadhaarVerificationsController> logger;
        AadhaarVerificationsService aadhaarverificationsService;
        public AadhaarVerificationsController(ILogger<AadhaarVerificationsController> logger, AadhaarVerificationsService aadhaarverificationsService)
        {
            logger = logger;
            this.aadhaarverificationsService = aadhaarverificationsService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<AadhaarVerifications>>> Entity()
        {
            ActionRes<AadhaarVerifications> result = new ActionRes<AadhaarVerifications>()
            {
               item = new AadhaarVerifications()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<AadhaarVerifications>>>> Select(ActionReq<AadhaarVerificationsSelectReq> req)
        {
            ActionRes<List<AadhaarVerifications>> result = new ActionRes<List<AadhaarVerifications>>();

            result.item = await aadhaarverificationsService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<AadhaarVerifications>>> Insert(ActionReq<AadhaarVerifications> req)
        {
            ActionRes<AadhaarVerifications> result = new ActionRes<AadhaarVerifications>();

            result.item = await aadhaarverificationsService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<AadhaarVerifications>>> Update(ActionReq<AadhaarVerifications> req)
        {
            ActionRes<AadhaarVerifications> result = new ActionRes<AadhaarVerifications>();

            result.item = await aadhaarverificationsService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<AadhaarVerifications>>> Save(ActionReq<AadhaarVerifications> req)
        {
            ActionRes<AadhaarVerifications> result = new ActionRes<AadhaarVerifications>();

            if(req.item.id > 0){
                result.item = await aadhaarverificationsService.Update(req.item);
            }else{
                result.item = await aadhaarverificationsService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<AadhaarVerificationsDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await aadhaarverificationsService.Delete(req.item);

            return Ok(result);
        }
    }
}
