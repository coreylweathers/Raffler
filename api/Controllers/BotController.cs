using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using shared.Models;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IRaffleService _raffleService;
        private readonly HubConnection _connection;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private string TempJsonString { get; set; }

        public BotController(IRaffleService service, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _raffleService = service;
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_config[Constants.SIGNAL_R_DOMAIN]}/rafflehub")
                .Build();
        }

        [HttpPost("entry")]
        public async Task<IActionResult> PostEntry([FromForm] string memory, [FromForm] string userIdentifier)
        {
            var json = JObject.Parse(memory);

            // TODO: Post memory data to Zapier link

            var entry = new RaffleEntry
            {
                MessageSid = json["twilio"]["sms"]["MessageSid"].ToString(),
                PhoneNumber = userIdentifier
            };

            JsonResult response = null;
            var googleData = GetGoogleSheetData(json["twilio"]["collected_data"]["collect_raffle_data"].ToString());
            try
            {
                var sid = await SendRaffleEntryToService(entry);
                await SendRaffleEntryToZapier(googleData.Name, googleData.Major, googleData.Classification, googleData.GradDate);
                response = NotifyRaffleEntrant("You are now entered into the raffle. We'll send you a text to let you know if you've won. Join us tonight and tomorrow for more exciting events by registering at https://twiliohbcu20x20atauc.splashthat.com/. Good luck 😉!");
            }
            catch (Exception)
            {
                // TODO: Log the error if an error occurs
                response = NotifyRaffleEntrant("We ran into a problem adding you to the raffle. Sit tight for a few minutes and try again afterwards.");
                throw;
            }
            finally
            {
                await _connection.StopAsync();
            }

            return response;

        }

        public (string Name, string Major, string Classification, string GradDate) GetGoogleSheetData(string memoryData)
        {
            var json = JObject.Parse(memoryData);
            string name = json["answers"]["full_name"]["answer"].ToString();
            string major = json["answers"]["major"]["answer"].ToString();
            string classification = json["answers"]["classification"]["answer"].ToString();
            string gradDate = json["answers"]["grad_date"]["answer"].ToString();

            return (name, major, classification, gradDate);
        }

        [HttpPost("intent")]
        public async Task<IActionResult> ConfirmIntent([FromForm] string memory, [FromForm] string userIdentifier, [FromForm] string channel)
        {
            if (string.IsNullOrEmpty(memory))
            {
                throw new ArgumentNullException(memory);
            }
            var json = JObject.Parse(memory);
            var answer = json["twilio"]["collected_data"]["collect_raffle_intent"]["answers"]["enter_raffle"]["answer"].ToString();
            string response = @"{""actions"": [{""redirect"":""task://enter_raffle""}]}";

            if (!await ShouldEnterRaffle(answer))
            {
                response = @"{""actions"": [{""redirect"":""task://dont_enter_raffle""}]}";
                return await Task.FromResult(new JsonResult(JObject.Parse(response)));
            }

            if (await IsProblemEntry(userIdentifier,channel))
            {
                return await Task.FromResult(new JsonResult(JObject.Parse(TempJsonString)));
            }

            var result = JObject.Parse(response);
            return await Task.FromResult(new JsonResult(result));
        }

        [HttpPost("inprogress")]
        public async Task<IActionResult> ConfirmRaffleState()
        {
            
            var redirectUri = (_raffleService.LatestRaffle.State == RaffleState.Running) ? "task://confirm_unique_entry" : "task://say_raffle_notstarted";
            
            var response = @"{""actions"": [{""redirect"":""" + redirectUri + @"""}]}";
            var result = JObject.Parse(response);

            return await Task.FromResult(new JsonResult(result));
        }

        private async Task<bool> ShouldEnterRaffle(string response)
        {
            if (!string.Equals("yes", response, StringComparison.CurrentCultureIgnoreCase))
            {
                return await Task.FromResult(false);
            }

            if (!await IsRaffleInProgress())
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
        private async Task<bool> IsRaffleInProgress()
        {
            if (_raffleService.LatestRaffle == null)
            {
                await _raffleService.InitializeService();
            }
            return await Task.FromResult(_raffleService.LatestRaffle.State == RaffleState.Running);
        }
        private async Task<bool> IsProblemEntry(string phoneNumber, string channel)
        {
            if (_raffleService.LatestRaffle == null)
            {
                await _raffleService.InitializeService();
            }

            if (!string.Equals(channel, "sms", StringComparison.CurrentCultureIgnoreCase))
            {
                TempJsonString = @"{""actions"": [{""say"":""Looks like you got to us from something other than SMS. Text in and let's see what happens.""}]}";
                return await Task.FromResult(true);
            }
            else if (await _raffleService.ContainsPhoneNumber(phoneNumber))
            {
                TempJsonString = @"{""actions"": [{""say"": ""Looks like you've already entered our raffle. Sit tight. We'll let you know if you've won""}]}";
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        private async Task SendRaffleEntryToZapier(string name, string major, string classification, string gradInfo)
        {
            // TODO: Update URI to grab the value from appsettings instead of hardcoding it
            var uri = new Uri("https://hooks.zapier.com/hooks/catch/3191324/ob7ijzi/");
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name",name),
                new KeyValuePair<string, string>("major", major),
                new KeyValuePair<string, string>("classification", classification),
                new KeyValuePair<string, string>("graddate", gradInfo)
            });

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.PostAsync(uri, data);
                response.EnsureSuccessStatusCode();
            }
        }
        private async Task<string> SendRaffleEntryToService(RaffleEntry entry)
        {
            var startConnectionTask = _connection.StartAsync();
            var sid = await _raffleService.AddRaffleEntry(entry);
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
