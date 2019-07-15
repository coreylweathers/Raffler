using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using shared.Models;
using shared.Services;
using System;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly HubConnection _connection;
        private readonly IConfiguration _config;

        public BotController(IStorageService service, IConfiguration configuration)
        {
            _storageService = service;
            _config = configuration;
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_config[Constants.SIGNALRDOMAIN]}/rafflehub")
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

            var response = @"{""actions"": [{""redirect"":""" + redirectUri + @"""}]}";
            var result = JObject.Parse(response);

            return new JsonResult(result);

        }

        [HttpPost("entry")]
        public async Task<IActionResult> PostEntryAsync([FromForm] string memory)
        {
            var json = JObject.Parse(memory);

            var entry = new RaffleEntry
            {
                MessageSid = json["twilio"]["sms"]["MessageSid"].ToString(),
                EmailAddress = json["twilio"]["collected_data"]["ask_email_address"]["answers"]["email_address"]["answer"].ToString()
            };

            var from = json["twilio"]["sms"]["To"].ToString();
            var to = json["twilio"]["sms"]["From"].ToString();

            var sid = string.Empty;
            try
            {
                sid = await SendRaffleEntryToService(entry);
                await NotifyRaffleEntrant(from, to, "You've been entered into the raffle. Good luck!");
            }
            catch (Exception)
            {
                // TODO: Log the error if an error occurrs
                await NotifyRaffleEntrant(from, to, "We ran into a problem adding you to the raffle. Sit tight for a few minutes and try again afterwards.");
                throw;
            }
            finally
            {
                await _connection.StopAsync();
            }

            return new OkObjectResult(sid);

        }

        [HttpPost("start")]
        public async Task<IActionResult> StartInteractionAsync()
        {
            var raffle = await _storageService.GetCurrentRaffleAsync();
            if (raffle.State == RaffleState.NotRunning)
            {
                var json = new
                {
                    actions = new[]
                    {
                        new { say  = "Thank you for texting into the app. We're not quite running a raffle at the moment but try again later and see if you can win a prize."}
                    }
                };
                return new JsonResult(json);
            }
            else
            {
                var json = new
                {
                    actions = new[]{ new
                   {
                       collect = new
                       {
                           name = "should_startraffle_yesno",
                           questions = new []
                           {
                               new
                               {
                                   question = "Did you want to enter the raffle?",
                                   name = "enter_raffle",
                                   type = "Twilio.YES_NO"
                               }
                           },
                           on_complete = new
                           {
                               redirect = new
                               {
                                   method = "POST",
                                   uri = $"{_config[Constants.APIDOMAIN]}/api/bot"
                               }
                           }
                       }
                   }
                }};

                return new JsonResult(json);
            }


        }

        private async Task<string> SendRaffleEntryToService(RaffleEntry entry)
        {
            var startConnectionTask = _connection.StartAsync();
            var sid = await _storageService.AddRaffleEntryAsync(entry);
            startConnectionTask.Wait();
            await _connection.InvokeAsync("AddRaffleEntry", entry);
            return sid;
        }
        private async Task NotifyRaffleEntrant(string from, string to, string body)
        {
            await MessageResource.CreateAsync(
                from: new PhoneNumber(from),
                to: new PhoneNumber(to),
                body: body);
        }
    }
}
