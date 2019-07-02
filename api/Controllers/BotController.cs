using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
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
        private readonly HubConnection _connection;

        public BotController(IStorageService service)
        {
            _storageService = service;
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:51379/rafflehub")
                .Build();
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
                MessageSid = json["twilio"]["sms"]["MessageSid"].ToString(),
                EmailAddress = json["twilio"]["collected_data"]["ask_email_address"]["answers"]["email_address"]["answer"].ToString()
            };

            var sid = string.Empty;
            try
            {
                await _connection.StartAsync();                
                sid = await _storageService.AddRaffleEntryAsync(entry);
                await _connection.InvokeAsync("AddNewRaffleEntry", entry);
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
