using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ILogger<UsersController> logger;
        UsersService usersService;
        public UsersController(ILogger<UsersController> logger, UsersService usersService)
        {
            logger = logger;
            this.usersService = usersService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<Users>>> Entity()
        {
            ActionRes<Users> result = new ActionRes<Users>()
            {
               item = new Users()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<Users>>>> Select(ActionReq<UsersSelectReq> req)
        {
            ActionRes<List<Users>> result = new ActionRes<List<Users>>();

            result.item = await usersService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<Users>>> Insert(ActionReq<Users> req)
        {
            ActionRes<Users> result = new ActionRes<Users>();

            result.item = await usersService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<Users>>> Update(ActionReq<Users> req)
        {
            ActionRes<Users> result = new ActionRes<Users>();

            result.item = await usersService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<Users>>> Save(ActionReq<Users> req)
        {
            ActionRes<Users> result = new ActionRes<Users>();

            if(req.item.id > 0){
                result.item = await usersService.Update(req.item);
            }else{
                result.item = await usersService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<UsersDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await usersService.Delete(req.item);

            return Ok(result);
        }
    }
}
