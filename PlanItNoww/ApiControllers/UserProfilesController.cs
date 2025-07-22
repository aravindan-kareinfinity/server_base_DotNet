using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        ILogger<UserProfilesController> logger;
        UserProfilesService userprofilesService;
        public UserProfilesController(ILogger<UserProfilesController> logger, UserProfilesService userprofilesService)
        {
            logger = logger;
            this.userprofilesService = userprofilesService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<UserProfiles>>> Entity()
        {
            ActionRes<UserProfiles> result = new ActionRes<UserProfiles>()
            {
               item = new UserProfiles()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<UserProfiles>>>> Select(ActionReq<UserProfilesSelectReq> req)
        {
            ActionRes<List<UserProfiles>> result = new ActionRes<List<UserProfiles>>();

            result.item = await userprofilesService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<UserProfiles>>> Insert(ActionReq<UserProfiles> req)
        {
            ActionRes<UserProfiles> result = new ActionRes<UserProfiles>();

            result.item = await userprofilesService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<UserProfiles>>> Update(ActionReq<UserProfiles> req)
        {
            ActionRes<UserProfiles> result = new ActionRes<UserProfiles>();

            result.item = await userprofilesService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<UserProfiles>>> Save(ActionReq<UserProfiles> req)
        {
            ActionRes<UserProfiles> result = new ActionRes<UserProfiles>();

            if(req.item.id > 0){
                result.item = await userprofilesService.Update(req.item);
            }else{
                result.item = await userprofilesService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<UserProfilesDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await userprofilesService.Delete(req.item);

            return Ok(result);
        }
    }
}
