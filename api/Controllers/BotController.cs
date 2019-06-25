using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QRCoder;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        // POST
        [HttpPost]
        public IActionResult Post([FromForm] string memory)
        {
            var redirectUri = "task://enter_raffle";
            var json = JObject.Parse(memory);
            var answer = json["twilio"]["collected_data"]["should_startraffle_yesno"]["answers"]["enter_raffle"]["answer"].ToString().ToLower();


            if (answer == "no")
            {
                redirectUri = "task://dont_enter_raffle";
            }

            var response = @"{""actions"": [{""redirect"":""" +redirectUri + @"""}]}";
            var result = JObject.Parse(response);

            return new JsonResult(result);

        }
    }
}
