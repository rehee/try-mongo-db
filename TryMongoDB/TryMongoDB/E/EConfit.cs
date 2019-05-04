using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
  public static partial class E
  {
    [Config]
    public static int MaxJoiner { get; set; }
    [Config]
    public static string StripePublicKey { get; set; }
    [Config]
    public static string StripePrivateKey { get; set; }
    [Config]
    public static bool UserNameIsNotEmail { get; set; }
  }
}

