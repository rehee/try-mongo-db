using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SDHC.Common.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TryMongoDB.MogoModels;
using TryMongoDB.MongoAuths;

namespace System
{
  public class MongoUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserEmailStore<TUser>, IUserPhoneNumberStore<TUser>, IUserTwoFactorStore<TUser, string>, IUserLoginStore<TUser, string>, IUserLockoutStore<TUser,string> where TUser : UserMongo, IUser<string>
  {
    public Task AddLoginAsync(TUser user, UserLoginInfo login)
    {
      throw new NotImplementedException();
    }

    public Task CreateAsync(TUser user)
    {
      var task = new Task(() =>
      {
        ModelManager.Create<TUser>(user);
      });
      task.Start();
      return task;

    }

    public Task DeleteAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
    }

    public Task<TUser> FindAsync(UserLoginInfo login)
    {
      throw new NotImplementedException();
    }

    public Task<TUser> FindByEmailAsync(string email)
    {
      var task = new Task<TUser>(() =>
      {
        var u = ModelManager.Read<UserMongo>(b => b.Email.ToLower() == email.ToLower()).FirstOrDefault();
        if (u == null)
        {
          return default(TUser);
        }
        return u as TUser;
      });
      task.Start();
      return task;
    }

    public Task<TUser> FindByIdAsync(string userId)
    {
      var task = new Task<TUser>(() =>
      {
        var u = ModelManager.Read<UserMongo>(b => b.Id == userId).FirstOrDefault();
        if (u == null)
        {
          return default(TUser);
        }
        return u as TUser;
      });
      task.Start();
      return task;
    }

    public Task<TUser> FindByNameAsync(string userName)
    {
      var task = new Task<TUser>(() =>
      {
        var u = ModelManager.Read<UserMongo>(b => b.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
        if (u == null)
        {
          return default(TUser);
        }
        return u as TUser;
      });
      task.Start();
      return task;
    }

    public Task<int> GetAccessFailedCountAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetEmailAsync(TUser user)
    {
      var task = new Task<string>(() =>
      {
        if (user == null)
        {
          return "";
        }
        return user.Email.ToLower();
      });
      task.Start();
      return task;
    }

    public Task<bool> GetEmailConfirmedAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<bool> GetLockoutEnabledAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
    {
      var task = new Task<IList<UserLoginInfo>>(() =>
      {
        var userId = user.Id;
        try
        {
          var userlogin = ModelManager.Read<UserLogin>(b => b.UserId == userId).ToList();
          return userlogin.Select(b => new UserLoginInfo(b.LoginProvider, b.ProviderKey)).ToList();
        }
        catch
        {
          var result = new List<UserLoginInfo>();
          return result;
        }
      });
      task.Start();
      return task;
    }

    public Task<string> GetPasswordHashAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetPhoneNumberAsync(TUser user)
    {
      var task = new Task<string>(() =>
      {
        return user?.PhoneNumber;
      });
      task.Start();
      return task;
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<bool> GetTwoFactorEnabledAsync(TUser user)
    {
      var task = new Task<bool>(() =>
      {
        return user != null ? user.TwoFactorEnabled : false;
      });
      task.Start();
      return task;
    }

    public Task<bool> HasPasswordAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<int> IncrementAccessFailedCountAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
    {
      throw new NotImplementedException();
    }

    public Task ResetAccessFailedCountAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task SetEmailAsync(TUser user, string email)
    {
      throw new NotImplementedException();
    }

    public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
    {
      throw new NotImplementedException();
    }

    public Task SetLockoutEnabledAsync(TUser user, bool enabled)
    {
      throw new NotImplementedException();
    }

    public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
    {
      throw new NotImplementedException();
    }

    public Task SetPasswordHashAsync(TUser user, string passwordHash)
    {

      var task = new Task(() =>
      {
        user.PasswordHash = passwordHash;
      });
      task.Start();
      return task;
    }

    public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
    {
      throw new NotImplementedException();
    }

    public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
    {
      throw new NotImplementedException();
    }

    public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
    {
      throw new NotImplementedException();
    }

    public Task UpdateAsync(TUser user)
    {
      throw new NotImplementedException();
    }
  }
}