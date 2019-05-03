using Microsoft.AspNet.Identity.EntityFramework;
using MongoDB.Driver;
using SDHC.Common.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TryMongoDB.Models;

namespace TryMongoDB.MogoModels
{

  public class MongoRepo : BaseMongoRepo<ApplicationUser>
  {
    public MongoRepo(string connectionString, string database) : base(connectionString, database)
    {
      //Class1s = new MongoDbSet<Class1>(this);
    }
    [MongoDbSetOption(ISetType = typeof(MongoDbSet<Class1>))]
    public IDbSet<Class1> Class1s { get; set; }
    [MongoDbSetOption(ISetType = typeof(MongoDbSet<Class2>))]
    public IDbSet<Class2> Class2s { get; set; }
  }
  public class BaseMongoRepo<TUser> : IdentityDbContext<TUser>, IBaseMongoRepo where TUser : IdentityUser, new()
  {
    public MongoClient Client { get; set; }
    public IMongoDatabase DataBase { get; set; }
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      Console.WriteLine("model create");
    }
    public BaseMongoRepo(string connectionString, string database)
    {
      Client = new MongoClient(connectionString);
      DataBase = Client.GetDatabase(database);
      var allProperty = this.GetType().GetProperties();
      var properties = this.GetType().GetProperties().Where(b => b.PropertyType.Name.Contains("IDbSet") || b.PropertyType.GetInterfaces().Any(i => i.Name.Contains("IDbSet"))).ToList();
      properties.ForEach(p =>
      {
        var attrs = p.GetObjectCustomAttribute<MongoDbSetOptionAttribute>();
        if (attrs != null && attrs.ISetType != null)
        {
          var table = attrs != null ? attrs.Table : "";
          var genericType = p.PropertyType.GenericTypeArguments.FirstOrDefault();
          var tableAttr = genericType.CustomAttributes.Where(b => b.AttributeType == typeof(TableAttribute)).FirstOrDefault();
          if (String.IsNullOrEmpty(table) && tableAttr != null)
          {
            table = tableAttr.ConstructorArguments.FirstOrDefault().Value as String;
          }

          p.SetValue(this, Activator.CreateInstance(attrs.ISetType, this, table));
        }

      });
      MongoDbIntKeyCountsCollection = DataBase.GetCollection<MongoDbIntKeyCount>("__MongoDbIntKeyCount");
      Users = new MongoDbSet<TUser>(this, "_user");
    }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<MongoDbIntKeyCount>), Table = "__MongoDbIntKeyCount")]
    public IDbSet<MongoDbIntKeyCount> MongoDbIntKeyCounts { get; set; }

    public List<Action> AddActionQue { get; set; } = new List<Action>();

    public override int SaveChanges()
    {
      AddActionQue.ForEach(a =>
      {
        try
        {
          a();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }

      });
      return 1;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
      var task = new Task<int>(() =>
     {
       return SaveChanges();
     });
      task.Start();
      return task;
    }

    public override Task<int> SaveChangesAsync()
    {
      return SaveChangesAsync(new CancellationToken());
    }
    #region

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<IdentityRole>), Table = "__Roles")]
    public override IDbSet<IdentityRole> Roles { get; set; }

    [MongoDbSetOption(Table = "__Users")]
    public override IDbSet<TUser> Users { get; set; }


    [MongoDbSetOption(ISetType = typeof(MongoDbSet<IdentityUserLogin>), Table = "__UserLogins")]
    public IDbSet<IdentityUserLogin> UserLogins { get; set; }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<IdentityUserRole>), Table = "__UserRoles")]
    public IDbSet<IdentityUserRole> UserRoles { get; set; }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<IdentityUserClaim>), Table = "__UserClaims")]
    public IDbSet<IdentityUserClaim> UserClaims { get; set; }


    public IMongoCollection<MongoDbIntKeyCount> MongoDbIntKeyCountsCollection { get; set; }
    #endregion
  }

  public interface IBaseMongoRepo : ISave
  {
    MongoClient Client { get; set; }
    IMongoDatabase DataBase { get; set; }
    List<Action> AddActionQue { get; set; }
    IDbSet<MongoDbIntKeyCount> MongoDbIntKeyCounts { get; set; }
    IMongoCollection<MongoDbIntKeyCount> MongoDbIntKeyCountsCollection { get; set; }
  }

  public class MongoDbIntKeyCount
  {
    public string Id { get; set; }
    public string TableName { get; set; }
    public Int64 KeyCount { get; set; } = 0;
    public MongoDbIntKeyCount()
    {
      if (String.IsNullOrEmpty(Id))
      {
        this.Id = Guid.NewGuid().ToString();
      }
    }
    public MongoDbIntKeyCount(string tableName) : this()
    {
      TableName = tableName;

    }
  }
  public class MongoDbSetOptionAttribute : Attribute
  {
    public string Table { get; set; } = "";
    public Type ISetType { get; set; } = null;
  }
}