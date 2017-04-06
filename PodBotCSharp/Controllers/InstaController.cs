using InstaSharp;
using InstaSharp.Models.Responses;
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
        public async Task<IHttpActionResult> Get()
        {
            return Index();
        }

        // GET: InstaAuth
        public IHttpActionResult Index()
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
            scopes.Add(InstaSharp.OAuth.Scope.Basic);
            scopes.Add(InstaSharp.OAuth.Scope.Likes);
            scopes.Add(InstaSharp.OAuth.Scope.Comments);

            var link = InstaSharp.OAuth.AuthLink(WebConfigurationManager.AppSettings["InstagramOAuthURL"] + "authorize", WebConfigurationManager.AppSettings["InstagramClientId"]
                , WebConfigurationManager.AppSettings["InstagramRedirectUri"], scopes, InstaSharp.OAuth.ResponseType.Code);

            return Redirect(link);
        }

        public async Task<IHttpActionResult> OAuth(string code)
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

            // all done, lets redirect to the home controller which will send some intial data to the app
            return Redirect("Index");
        }
    }
}
