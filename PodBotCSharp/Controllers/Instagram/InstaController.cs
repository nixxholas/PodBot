using InstaSharp;
using InstaSharp.Models.Responses;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace PodBotCSharp.Controllers.Instagram
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
    }
}
