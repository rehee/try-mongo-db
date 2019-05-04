using Microsoft.AspNet.Identity;
using SDHC.Common.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TryMongoDB.MogoModels;

namespace System
{
  public class MongoUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser> where TUser : class, IUser<string>
  {
    public Task CreateAsync(TUser user)
    {
      var task = new Task(()=> 
      {

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
      throw new NotImplementedException();
    }

    public Task<TUser> FindByIdAsync(string userId)
    {
      throw new NotImplementedException();
    }

    public Task<TUser> FindByNameAsync(string userName)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetPasswordHashAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task<bool> HasPasswordAsync(TUser user)
    {
      throw new NotImplementedException();
    }

    public Task SetPasswordHashAsync(TUser user, string passwordHash)
    {
      throw new NotImplementedException();
    }

    public Task UpdateAsync(TUser user)
    {
      throw new NotImplementedException();
    }
  }
}