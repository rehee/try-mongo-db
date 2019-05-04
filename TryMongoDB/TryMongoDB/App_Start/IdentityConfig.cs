using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SDHC.Common.Entity.Models;
using TryMongoDB.MongoAuths;

namespace System
{
  public class ApplicationRoleManagerMongo : RoleManager<IdentityRole>
  {
    public ApplicationRoleManagerMongo(IRoleStore<IdentityRole, string> store) : base(store)
    {

    }
    public static ApplicationRoleManagerMongo Create<T>(IdentityFactoryOptions<ApplicationRoleManagerMongo> options, IOwinContext context) where T : DbContext
    {
      return new ApplicationRoleManagerMongo(new RoleStore<IdentityRole>(context.Get<T>()));
    }
  }
  // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
  public class ApplicationUserManagerMongo : UserManager<UserMongo>
  {

    public ApplicationUserManagerMongo(IUserStore<UserMongo> store)
        : base(store)
    {
    }

    
    public static ApplicationUserManagerMongo Create<T>(IdentityFactoryOptions<ApplicationUserManagerMongo> options, IOwinContext context) where T : DbContext
    {
      ApplicationUserManagerMongo manager;
      if (G.MongoDbIuserStore != null)
      {
        
        
      }
      else
      {
        //manager = new ApplicationUserManagerMongo(new UserStore<SDHCUser>(context.Get<T>()));
      }
      var s = new MongoUserStore<UserMongo>();
      manager = new ApplicationUserManagerMongo(s);
      //manager = new ApplicationUserManagerMongo(new MUserStore<UserMongo>(context.Get<T>()));
      // Configure validation logic for usernames
      manager.UserValidator = new UserValidator<UserMongo>(manager)
      {
        AllowOnlyAlphanumericUserNames = false,
        RequireUniqueEmail = true
      };

      // Configure validation logic for passwords
      manager.PasswordValidator = new PasswordValidator
      {
        RequiredLength = 1,
        RequireNonLetterOrDigit = false,
        RequireDigit = false,
        RequireLowercase = false,
        RequireUppercase = false,
      };

      // Configure user lockout defaults
      manager.UserLockoutEnabledByDefault = false;
      manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
      manager.MaxFailedAccessAttemptsBeforeLockout = 99999;

      // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
      // You can write your own provider and plug it in here.
      manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<UserMongo>
      {
        MessageFormat = "Your security code is {0}"
      });
      manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<UserMongo>
      {
        Subject = "Security Code",
        BodyFormat = "Your security code is {0}"
      });
      manager.EmailService = new EmailService();
      manager.SmsService = new SmsService();
      var dataProtectionProvider = options.DataProtectionProvider;
      if (dataProtectionProvider != null)
      {
        manager.UserTokenProvider =
            new DataProtectorTokenProvider<UserMongo>(dataProtectionProvider.Create("ASP.NET Identity"));
      }
      return manager;
    }
  }

  // Configure the application sign-in manager which is used in this application.
  public class ApplicationSignInManagerMongo : SignInManager<UserMongo, string>
  {
    public ApplicationSignInManagerMongo(ApplicationUserManagerMongo userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
    {
    }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(UserMongo user)
    {
      return user.GenerateUserIdentityAsync((ApplicationUserManagerMongo)UserManager);
    }

    public static ApplicationSignInManagerMongo Create(IdentityFactoryOptions<ApplicationSignInManagerMongo> options, IOwinContext context)
    {
      return new ApplicationSignInManagerMongo(context.GetUserManager<ApplicationUserManagerMongo>(), context.Authentication);
    }
  }
}

namespace Start
{

}
