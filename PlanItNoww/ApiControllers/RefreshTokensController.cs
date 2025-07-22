using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokensController : ControllerBase
    {
        ILogger<RefreshTokensController> logger;
        RefreshTokensService refreshtokensService;
        public RefreshTokensController(ILogger<RefreshTokensController> logger, RefreshTokensService refreshtokensService)
        {
            logger = logger;
            this.refreshtokensService = refreshtokensService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<RefreshTokens>>> Entity()
        {
            ActionRes<RefreshTokens> result = new ActionRes<RefreshTokens>()
            {
               item = new RefreshTokens()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<RefreshTokens>>>> Select(ActionReq<RefreshTokensSelectReq> req)
        {
            ActionRes<List<RefreshTokens>> result = new ActionRes<List<RefreshTokens>>();

            result.item = await refreshtokensService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<RefreshTokens>>> Insert(ActionReq<RefreshTokens> req)
        {
            ActionRes<RefreshTokens> result = new ActionRes<RefreshTokens>();

            result.item = await refreshtokensService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<RefreshTokens>>> Update(ActionReq<RefreshTokens> req)
        {
            ActionRes<RefreshTokens> result = new ActionRes<RefreshTokens>();

            result.item = await refreshtokensService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<RefreshTokens>>> Save(ActionReq<RefreshTokens> req)
        {
            ActionRes<RefreshTokens> result = new ActionRes<RefreshTokens>();

            if(req.item.id > 0){
                result.item = await refreshtokensService.Update(req.item);
            }else{
                result.item = await refreshtokensService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<RefreshTokensDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await refreshtokensService.Delete(req.item);

            return Ok(result);
        }
    }
}
