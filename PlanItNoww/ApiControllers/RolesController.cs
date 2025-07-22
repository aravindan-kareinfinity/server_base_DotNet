using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        ILogger<RolesController> logger;
        RolesService rolesService;
        public RolesController(ILogger<RolesController> logger, RolesService rolesService)
        {
            logger = logger;
            this.rolesService = rolesService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<Roles>>> Entity()
        {
            ActionRes<Roles> result = new ActionRes<Roles>()
            {
               item = new Roles()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<Roles>>>> Select(ActionReq<RolesSelectReq> req)
        {
            ActionRes<List<Roles>> result = new ActionRes<List<Roles>>();

            result.item = await rolesService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<Roles>>> Insert(ActionReq<Roles> req)
        {
            ActionRes<Roles> result = new ActionRes<Roles>();

            result.item = await rolesService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<Roles>>> Update(ActionReq<Roles> req)
        {
            ActionRes<Roles> result = new ActionRes<Roles>();

            result.item = await rolesService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<Roles>>> Save(ActionReq<Roles> req)
        {
            ActionRes<Roles> result = new ActionRes<Roles>();

            if(req.item.id > 0){
                result.item = await rolesService.Update(req.item);
            }else{
                result.item = await rolesService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<RolesDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await rolesService.Delete(req.item);

            return Ok(result);
        }
    }
}
