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
using TryMongoDB.MongoAuths;

namespace TryMongoDB.MogoModels
{

  public class MongoRepo : BaseMongoRepo<UserMongo>
  {
    public MongoRepo() : base()
    {
      this.connectionString = @"mongodb://localhost:27017";
      this.database = "lalala";
      Init();
    }
    public static MongoRepo Create()
    {
      return new MongoRepo();
    }
    [MongoDbSetOption(ISetType = typeof(MongoDbSet<Class1>))]
    public IDbSet<Class1> Class1s { get; set; }
    [MongoDbSetOption(ISetType = typeof(MongoDbSet<Class2>), Table = "Content1")]
    public IDbSet<Class2> Class2s { get; set; }
    [MongoDbSetOption(ISetType = typeof(MongoDbSet<SDHCRoot>), Table = "Content2")]
    public IDbSet<SDHCRoot> SDHCRoots { get; set; }
  }
  public class BaseMongoRepo<TUser> : IdentityMongoDB<TUser>, IBaseMongoRepo where TUser : UserMongo, new()
  {
    public MongoClient Client { get; set; }
    public IMongoDatabase DataBase { get; set; }
    public string connectionString { get; set; } = "";
    public string database { get; set; } = "";
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      Console.WriteLine("model create");
    }
    public void Init()
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
    public BaseMongoRepo()
    {

    }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<MongoDbIntKeyCount>), Table = "__MongoDbIntKeyCount")]
    public IDbSet<MongoDbIntKeyCount> MongoDbIntKeyCounts { get; set; }

    public List<Action> ActionList { get; set; } = new List<Action>();
    public override int SaveChanges()
    {
      ActionList.ForEach(a =>
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
      ActionList.Clear();
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
    protected override bool ShouldValidateEntity(DbEntityEntry entityEntry)
    {
      return false;
    }
    protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
    {
      return base.ValidateEntity(entityEntry, items);
    }
    public override Task<int> SaveChangesAsync()
    {
      return SaveChangesAsync(new CancellationToken());
    }
    #region

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<MongoRole>), Table = "__Roles")]
    public override IDbSet<MongoRole> Roles { get; set; }

    [MongoDbSetOption(Table = "__Users")]
    public override IDbSet<TUser> Users { get; set; }


    [MongoDbSetOption(ISetType = typeof(MongoDbSet<UserLogin>), Table = "__UserLogins")]
    public IDbSet<UserLogin> UserLogins { get; set; }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<UserRole>), Table = "__UserRoles")]
    public IDbSet<UserRole> UserRoles { get; set; }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<UserClaim>), Table = "__UserClaims")]
    public IDbSet<UserClaim> UserClaims { get; set; }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<BaseContent>), Table = "Content")]
    public IDbSet<BaseContent> Contents { get; set; }

    [MongoDbSetOption(ISetType = typeof(MongoDbSet<BaseSelect>), Table = "UserSelect")]
    public IDbSet<BaseSelect> Selects { get; set; }

    public IMongoCollection<MongoDbIntKeyCount> MongoDbIntKeyCountsCollection { get; set; }
    #endregion
  }

  public interface IBaseMongoRepo : IContent
  {
    MongoClient Client { get; set; }
    IMongoDatabase DataBase { get; set; }
    List<Action> ActionList { get; set; }
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

namespace Microsoft.AspNet.Identity.EntityFramework
{
  public class MongoRole: IdentityRole<string, UserRole>
  {
  }
  public class UserLogin : IdentityUserLogin
  {
    [Key]
    public string Id { get; set; }
  }
  public class UserRole : IdentityUserRole
  {
    [Key]
    public string Id { get; set; }
  }
  public class UserClaim :  IdentityUserClaim
  {
    
  }
}