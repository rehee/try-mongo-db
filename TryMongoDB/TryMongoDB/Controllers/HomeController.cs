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
    public ActionResult Index()
    {
      var dbConnect = @"mongodb+srv://rehee_1:rehee_1_psw@cluster0-igkz0.gcp.mongodb.net/test?retryWrites=true";
      var dbName = "lalala";
      var db = new MongoRepo(dbConnect, dbName);
      var start = DateTime.Now;
      //for (var i = 0; i < 10; i++)
      //{
      //  var class2 = new Class2();
      //  db.Class2s.Add(class2);
      //  db.SaveChanges();
      //}
      //var d = new ApplicationDbContext();
      var classes = db.Class2s.ToList();
      classes.ForEach(b =>
      {
        db.Class2s.Remove(b);
        db.SaveChanges();
      });
      var end = DateTime.Now;
      //var table = "Class2";
      //var lists = db.MongoDbIntKeyCounts.Where(b => b.TableName == table).ToList().FirstOrDefault();
      //lists.KeyCount++;
      //var bson = lists.ToBsonDocument();
      ////bson["_id"] = lists.Id;
      //bson.Remove("_id");
      //db.MongoDbIntKeyCountsCollection.UpdateOne<MongoDbIntKeyCount>(b => b.TableName == table, new BsonDocument { { "$set", bson } });
      return Content(classes.Count.ToString() + " " + (end - start).Seconds.ToString());
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