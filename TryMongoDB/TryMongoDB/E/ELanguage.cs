using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
  public static partial class E
  {
    [Config]
    public static int DefaultLanguage { get; set; }
    public static void SetLang(EnumLang lang)
    {
      LanguageManager.SetLang((int)lang);
    }
    public static EnumLang GetLang()
    {
      return (EnumLang)LanguageManager.GetLang();
    }
    public static Func<string, string, string> TextCnEn = (cn, en) =>
      {
        switch (GetLang())
        {
          case EnumLang.Cn:
            return cn;
          case EnumLang.En:
            return en;
          default:
            return cn;
        }
      };
  }

  public enum EnumLang
  {
    Cn = 0,
    En = 1
  }
}