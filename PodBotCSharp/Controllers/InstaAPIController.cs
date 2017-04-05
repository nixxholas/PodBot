using InstaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace PodBotCSharp.Controllers
{
    public class InstaAPIController : Controller
    {
        // GET: InstaAuth
        public ActionResult Index()
        {
            return View();
        }

        // GET: InstaAuth/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InstaAuth/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Login()
        {
            // Create a scope that define what we're gonna use
            var scopes = new List<OAuth.Scope>();
            scopes.Add(OAuth.Scope.Basic);
            scopes.Add(OAuth.Scope.Likes);
            scopes.Add(OAuth.Scope.Comments);

            var link = OAuth.AuthLink(WebConfigurationManager.AppSettings["InstagramOAuthURL"] + "authorize", WebConfigurationManager.AppSettings["InstagramClientId"]
                , WebConfigurationManager.AppSettings["InstagramRedirectUri"], scopes, OAuth.ResponseType.Code);

            return Redirect(link);
        }

        // POST: InstaAuth/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: InstaAuth/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InstaAuth/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: InstaAuth/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InstaAuth/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
