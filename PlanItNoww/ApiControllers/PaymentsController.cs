using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        ILogger<PaymentsController> logger;
        PaymentsService paymentsService;
        public PaymentsController(ILogger<PaymentsController> logger, PaymentsService paymentsService)
        {
            logger = logger;
            this.paymentsService = paymentsService;
        }
        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<Payments>>> Entity()
        {
            ActionRes<Payments> result = new ActionRes<Payments>()
            {
               item = new Payments()
            };

            return Ok(result);
        }
        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<Payments>>>> Select(ActionReq<PaymentsSelectReq> req)
        {
            ActionRes<List<Payments>> result = new ActionRes<List<Payments>>();

            result.item = await paymentsService.Select(req.item);

            return Ok(result);
        }
        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<Payments>>> Insert(ActionReq<Payments> req)
        {
            ActionRes<Payments> result = new ActionRes<Payments>();

            result.item = await paymentsService.Insert(req.item);

            return Ok(result);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<Payments>>> Update(ActionReq<Payments> req)
        {
            ActionRes<Payments> result = new ActionRes<Payments>();

            result.item = await paymentsService.Update(req.item);

            return Ok(result);
        }
        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<Payments>>> Save(ActionReq<Payments> req)
        {
            ActionRes<Payments> result = new ActionRes<Payments>();

            if(req.item.id > 0){
                result.item = await paymentsService.Update(req.item);
            }else{
                result.item = await paymentsService.Insert(req.item);
            }

            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<PaymentsDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await paymentsService.Delete(req.item);

            return Ok(result);
        }
    }
}
