using SDHC.Common.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TryMongoDB.Models
{
  public static class Containers
  {
    public static Dictionary<string, object> d { get; set; } = new Dictionary<string, object>();
  }
  [Table("Class1")]
  public class Class1: IInt64Key
  {
    [Key]
    public Int64 Id { get; set; }
    
    public string Title { get; set; }
    
  }

  [Table("Class2")]
  public class Class2: IInt64Key
  {
    public Class2()
    {

    }
    [Key]
    public Int64 Id { get; set; }
    public string Title { get; set; }
    public string Title2 { get; set; }
  }


}