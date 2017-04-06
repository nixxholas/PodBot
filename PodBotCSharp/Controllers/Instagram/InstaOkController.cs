using InstaSharp;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace PodBotCSharp.Controllers.Instagram
{
    // Receiving Controller for InstaController
    public class InstaOkController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetAsync([FromBody]string code)
        {
            // add this code to the auth object
            var auth = new OAuth(new InstagramConfig(WebConfigurationManager.AppSettings["InstagramClientId"], WebConfigurationManager.AppSettings["InstagramClientSecret"]
                , WebConfigurationManager.AppSettings["InstagramRedirectUri"], ""));

            // now we have to call back to instagram and include the code they gave us
            // along with our client secret
            var oauthResponse = await auth.RequestToken(code);

            // both the client secret and the token are considered sensitive data, so we won't be
            // sending them back to the browser. we'll only store them temporarily.  If a user's session times
            // out, they will have to click on the authenticate button again - sorry bout yer luck.
            // http://stackoverflow.com/questions/11478244/asp-net-web-api-session-or-something
            HttpContext.Current.Session.Add("InstaSharp.AuthInfo", oauthResponse);

            // Store access token to bot state
            ///// Here we store the only access token.
            ///// Please store refresh token, too.
            var botCred = new MicrosoftAppCredentials(
                WebConfigurationManager.AppSettings["MicrosoftAppId"],
                WebConfigurationManager.AppSettings["MicrosoftAppPassword"]);
            var stateClient = new StateClient(botCred);
            BotState botState = new BotState(stateClient);
            BotData botData = new BotData(eTag: "*");
            botData.SetProperty("igAccessToken", code);
            await stateClient.BotState.SetUserDataAsync("telegram", code, botData);

            // all done, lets return ok
            return Ok();
        }
    }
}
