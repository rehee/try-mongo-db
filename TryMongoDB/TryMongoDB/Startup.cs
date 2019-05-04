using Microsoft.Owin;
using Owin;
using SDHC.Common.Entity.Models;
using Start;
using System;
using System.Web.Hosting;
using TryMongoDB.Models;
using TryMongoDB.MogoModels;
using TryMongoDB.MongoAuths;

[assembly: OwinStartupAttribute(typeof(TryMongoDB.Startup))]
namespace TryMongoDB
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      G.MongoDbIuserStore = () => null;
      SDHCStartup2.Init<MongoRepo, SDHCRoot, SDHCBascSelect, UserMongo>(
       app, () => MongoRepo.Create(), HostingEnvironment.MapPath("/"));
    }
  }
}
