using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TryMongoDB.MongoAuths
{
  public class MUserStore<TUser> : UserStore<TUser, MongoRole, string, UserLogin, UserRole, UserClaim>, IUserStore<TUser>, IUserStore<TUser, string>, IDisposable
    where TUser : UserMongo
  {
    public MUserStore(DbContext context) : base(context)
    {
    }
  }

  public class IdentityMongoDB<TUser> : IdentityDbContext<TUser, MongoRole, string, UserLogin, UserRole, UserClaim>
    where TUser : IdentityUser<string, UserLogin, UserRole, UserClaim>
  {

  }
  public class UserMongo : IdentityUser<string, UserLogin, UserRole, UserClaim>
  {
    public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserMongo> manager)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      // Add custom user claims here
      return userIdentity;
    }

    [NotMapped]
    [IgnoreEdit]
    public override ICollection<UserRole> Roles { get;}
    [NotMapped]
    [IgnoreEdit]
    public override ICollection<UserClaim> Claims { get; }
    [NotMapped]
    [IgnoreEdit]
    public override ICollection<UserLogin> Logins { get; }
  }
}