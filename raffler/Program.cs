using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
// ReSharper disable ClassNeverInstantiated.Global

namespace raffler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
.ConfigureWebHostDefaults(webBuilder =>
{
    if (string.Equals(webBuilder.GetSetting("Environment"), "Development", System.StringComparison.CurrentCultureIgnoreCase))
    {
        webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
    }
    webBuilder.UseStartup<Startup>();
});
        }
    }
}
