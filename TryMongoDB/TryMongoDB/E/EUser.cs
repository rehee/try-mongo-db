using Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
  public static partial class E
  {
    public static Func<ApplicationUserManager> UserManager { get; set; }
    public static Func<ApplicationSignInManager> SignManager { get; set; }
    public static Func<ApplicationRoleManager> RoleManager { get; set; }
  }
}