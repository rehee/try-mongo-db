﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Areas.Admin.Controllers
{
  public class DashBoardController : Controller
  {
    // GET: Admin/Home
    public ActionResult Index()
    {
      return View();
    }
  }
}