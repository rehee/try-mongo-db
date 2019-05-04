using Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace System
{
  public static partial class E
  {
    public static void Init()
    {
      #region Default Start Up
      BeforeStart.Init(key =>
      {
        var setting = WebConfigurationManager.AppSettings[key];
        return setting.Text();
      }, typeof(E),
      (key) => HttpContext.Current.Session[key],
      (key, value) => HttpContext.Current.Session[key] = value,
      () => HttpContext.Current.Request.GetOwinContext());
      #endregion


    }
  }
}