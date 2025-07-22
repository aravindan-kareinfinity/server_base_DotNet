using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsReferenceController : ControllerBase
    {
        ILogger<PermissionsReferenceController> logger;
        PermissionsReferenceService permissionsreferenceService;
        public PermissionsReferenceController(ILogger<PermissionsReferenceController> logger, PermissionsReferenceService permissionsreferenceService)
        {
            logger = logger;
            this.permissionsreferenceService = permissionsreferenceService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<PermissionsReference>>> Entity()
        {
            ActionRes<PermissionsReference> result = new ActionRes<PermissionsReference>()
            {
               item = new PermissionsReference()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<PermissionsReference>>>> Select(ActionReq<PermissionsReferenceSelectReq> req)
        {
            ActionRes<List<PermissionsReference>> result = new ActionRes<List<PermissionsReference>>();

            result.item = await permissionsreferenceService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<PermissionsReference>>> Insert(ActionReq<PermissionsReference> req)
        {
            ActionRes<PermissionsReference> result = new ActionRes<PermissionsReference>();

            result.item = await permissionsreferenceService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<PermissionsReference>>> Update(ActionReq<PermissionsReference> req)
        {
            ActionRes<PermissionsReference> result = new ActionRes<PermissionsReference>();

            result.item = await permissionsreferenceService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<PermissionsReference>>> Save(ActionReq<PermissionsReference> req)
        {
            ActionRes<PermissionsReference> result = new ActionRes<PermissionsReference>();

            if(req.item.id > 0){
                result.item = await permissionsreferenceService.Update(req.item);
            }else{
                result.item = await permissionsreferenceService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<PermissionsReferenceDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await permissionsreferenceService.Delete(req.item);

            return Ok(result);
        }
    }
}
