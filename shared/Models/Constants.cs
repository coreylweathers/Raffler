namespace shared.Models
{
    // TODO: Convert Constants over to IOptions approach (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.2)
    public static class Constants
    {
        public const string ACCOUNT_SID_PATH = "Values:Twilio:AccountSid";
        public const string SYNC_SERVICE_SID_PATH = "Values:Twilio:SyncServiceSid";
        public const string AUTH_TOKEN_PATH = "TwilioAuthToken";
        public const string API_DOMAIN = "Values:Domain:Api";
        public const string SIGNAL_R_DOMAIN = "Values:Domain:Signalr";
        public const string SYNC_SERVICE_LIST_SID_PATH = "Values:Twilio:SyncServiceListSid";
    }
}
