using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TryMongoDB.Models;
using TryMongoDB.MogoModels;

namespace TryMongoDB.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index(long? id)
    {
      var db = new MongoRepo();
      var d = new ApplicationDbContext();
      var class1 = new Class2();
      ContentManager.CreateContent(class1);
      return Content($"");
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}