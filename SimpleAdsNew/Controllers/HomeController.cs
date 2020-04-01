﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleAdsNew.Data;
using SimpleAdsNew.Models;

namespace SimpleAdsNew.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
            "Data Source=.\\sqlexpress;Initial Catalog=SimpleAds;Integrated Security=True";
        public IActionResult Index()
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            IEnumerable<SimpleAd> ads = db.GetAds();
            List<string> ids = new List<string>();
            if (Request.Cookies["AdIds"] != null)
            {
                ids = Request.Cookies["AdIds"].Split(',').ToList();
            }

            var vm = new HomePageViewModel
            {
                Ads = ads.Select(ad => new AdViewModel
                {
                    Ad = ad,
                    CanDelete = ids.Contains(ad.Id.ToString())
                })
            };
            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["Message"];
            }


            return View(vm);
        }

        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewAd(SimpleAd ad)
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            db.AddSimpleAd(ad);
            string ids = "";
            var cookie = Request.Cookies["AdIds"];
            if (cookie != null)
            {
                ids = $"{cookie},";
            }
            ids += ad.Id;
            Response.Cookies.Append("AdIds", ids);

            return Redirect("/");
        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            db.Delete(id);
            return Redirect("/");
        }
    }

    
}
