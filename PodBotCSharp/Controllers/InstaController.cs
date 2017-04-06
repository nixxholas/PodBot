using InstaSharp;
using InstaSharp.Models.Responses;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace PodBotCSharp.Controllers
{
    public class InstaController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            // Sessions
            // http://stackoverflow.com/questions/11478244/asp-net-web-api-session-or-something
            if (HttpContext.Current.Session["InstaSharp.AuthInfo"] != null)
            {
                var oAuthResponse = HttpContext.Current.Session["InstaSharp.AuthInfo"] as OAuthResponse;

                return Ok(oAuthResponse.User);
            }
            else
            {
                return Login();
            }
        }

        // GET: InstaAuth/Create
        public IHttpActionResult Create()
        {
            // Sessions
            // http://stackoverflow.com/questions/11478244/asp-net-web-api-session-or-something
            var oAuthResponse = HttpContext.Current.Session["InstaSharp.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return Login();
            }

            return Ok(oAuthResponse.User);
        }

        public IHttpActionResult Login()
        {
            // Create a scope that define what we're gonna use
            var scopes = new List<OAuth.Scope>();
            scopes.Add(OAuth.Scope.Basic);
            scopes.Add(OAuth.Scope.Likes);
            scopes.Add(OAuth.Scope.Comments);
            scopes.Add(OAuth.Scope.Public_Content);
            scopes.Add(OAuth.Scope.Follower_List);
            scopes.Add(OAuth.Scope.Relationships);

            var link = OAuth.AuthLink(WebConfigurationManager.AppSettings["InstagramOAuthURL"] + "authorize", WebConfigurationManager.AppSettings["InstagramClientId"]
                , WebConfigurationManager.AppSettings["InstagramRedirectUri"], scopes, InstaSharp.OAuth.ResponseType.Code);

            return Redirect(link);
        }
        
        public async Task<IHttpActionResult> Get([FromBody]string code)
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

            // all done, lets redirect to the home controller which will send some intial data to the app
            return Redirect("Index");
        }
        
    }
}
