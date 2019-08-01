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
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IRaffleService _raffleService;
        private readonly HubConnection _connection;
        private readonly IConfiguration _config;

        public BotController(IRaffleService service, IConfiguration configuration)
        {
            _raffleService = service;
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
            JsonResult response = null;
            try
            {
                string sid = await SendRaffleEntryToService(entry);
                response = NotifyRaffleEntrant("You've been entered into the raffle. Good luck!");
            }
            catch (Exception)
            {
                // TODO: Log the error if an error occurrs
                response = NotifyRaffleEntrant("We ran into a problem adding you to the raffle. Sit tight for a few minutes and try again afterwards.");
                throw;
            }
            finally
            {
                await _connection.StopAsync();
            }

            return response;

        }

        [HttpPost("start")]
        public async Task<IActionResult> StartInteractionAsync()
        {
            var raffle = await _raffleService.GetCurrentRaffleAsync();
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
                                   question = "Hi and Thank you for stopping by the Twilio booth at KCDC. Did you want to enter the raffle? Text in yes or no",
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
            var sid = await _raffleService.AddRaffleEntryAsync(entry);
            startConnectionTask.Wait();
            await _connection.InvokeAsync("AddRaffleEntry", entry);
            return sid;
        }
        private JsonResult NotifyRaffleEntrant(string body)
        {

            var json = new
            {
                actions = new[]
                {
                    new
                    {
                        say = body
                    }
                }
            };

            return new JsonResult(json);
        }
    }
}
