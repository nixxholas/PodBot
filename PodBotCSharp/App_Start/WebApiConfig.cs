using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Configuration;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using NetTelegramBotApi;
using PodBotCSharp.Models;

namespace PodBotCSharp
{
    public static class WebApiConfig
    {
        // TelegramBotClient Handling
        // Retrieving an appsetting from the web.config
        // http://stackoverflow.com/questions/4595288/reading-a-key-from-the-web-config-using-configurationmanager
        public static readonly TelegramBot TelegramHook = new TelegramBot(WebConfigurationManager.AppSettings["TelegramBotToken"]);
        // InstagramClient Handling
        // InstagramConfig config = new InstagramConfig(WebConfigurationManager.AppSettings["InstagramClientId"]
        //, WebConfigurationManager.AppSettings["InstagramClientId"], WebConfigurationManager.AppSettings["InstagramRedirectUri"]
        //, WebConfigurationManager.AppSettings["InstagramRealtimeUri"]);

        public static readonly ChannelAccount botTelegramAccount = new ChannelAccount(WebConfigurationManager.AppSettings["TelegramChannelId"], "PodBot");

        public static Dictionary<string, UserChannelData> UserBase = new Dictionary<string, UserChannelData>();

        public static void Register(HttpConfiguration config)
        {
            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    action = RouteParameter.Optional
                }
            );
        }
    }
}
