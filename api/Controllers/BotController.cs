using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using shared.Models;
using shared.Services;
using System;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public BotController(IStorageService service)
        {
            _storageService = service;
        }
        // POST
        [HttpPost("")]
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

        [HttpPost("entry")]
        public async Task<IActionResult> PostEntryAsync([FromForm] string memory)
        {
            var json = JObject.Parse(memory);
            var fromNumber = json["twilio"]["sms"]["From"].ToString();

            var entry = new RaffleEntry
            {
                Digits = fromNumber.Substring(fromNumber.Length - 7, 7),
                MessageSid = json["twilio"]["sms"]["MessageSid"].ToString(),
                EmailAddress = json["twilio"]["collected_data"]["ask_email_address"]["answers"]["email_address"]["answer"].ToString()
            };

            var sid = string.Empty;
            try
            {
                sid = await _storageService.AddRaffleEntryAsync(entry);

            }
            catch (Exception ex)
            {
                // TODO: Log the error if an error occurrs
                sid = ex.Message;
            }

            return new OkObjectResult(sid);

        }
    }
}
