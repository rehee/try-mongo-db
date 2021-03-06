﻿using MongoDB.Bson;
using MongoDB.Driver;
using SDHC.Common.Entity.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace TryMongoDB.MogoModels
{
  public interface IUpdate
  {
    void Update(object entity);
  }
  public class MongoDbSet<T> : IUpdate, IDbSet<T> where T : class
  {
    private IMongoDatabase db;
    private IQueryable<T> thisQ { get; set; }
    private IBaseMongoRepo repo { get; set; }
    private IMongoCollection<T> collection { get; set; }
    private IMongoCollection<BsonDocument> collectionBson { get; set; }
    public string table { get; set; }
    public MongoDbSet(IBaseMongoRepo repo, string table = "")
    {
      this.repo = repo;
      this.db = repo.DataBase;

      if (String.IsNullOrEmpty(table))
      {
        table = this.GetType().GenericTypeArguments.FirstOrDefault().Name;
      }
      this.table = table;
      collection = db.GetCollection<T>(table);
      collectionBson = db.GetCollection<BsonDocument>(table);
      thisQ = db.GetCollection<T>(table).AsQueryable();
    }
    public ObservableCollection<T> Local => new ObservableCollection<T>();
   
    public Expression Expression => this.thisQ.Expression;

    public Type ElementType => thisQ.ElementType;

    public IQueryProvider Provider => thisQ.Provider;

    private MongoDbIntKeyCount CurrentIndexCount
    {
      get
      {
        var tableName = this.table;
        MongoDbIntKeyCount count = this.repo.MongoDbIntKeyCounts.Count() == 0 ? null : this.repo.MongoDbIntKeyCounts.Where(b => b.TableName == tableName).ToList().FirstOrDefault();
        if (count == null)
        {
          count = new MongoDbIntKeyCount(tableName);
          repo.MongoDbIntKeyCountsCollection.InsertOne(count);
        }
        return count;
      }
    }
    private void AddCurrentIndex(MongoDbIntKeyCount next)
    {
      var tableName = this.table;
      var bson = next.ToBsonDocument();
      bson.Remove("_id");
      bson.Remove("TableName");
      this.repo.MongoDbIntKeyCountsCollection.UpdateOne<MongoDbIntKeyCount>(b => b.TableName == tableName, new BsonDocument { { "$set", bson } });
    }
    public T Add(T entity)
    {
      var thisType = typeof(T);
      if (thisType == typeof(MongoDbIntKeyCount))
        return entity;
      var tableName = this.table;
      Action action = () =>
      {
        try
        {
          var isKeyLong = thisType.GetInterfaces().Any(i => i == typeof(IInt64Key));
          MongoDbIntKeyCount currentLongIndex = null;
          if (isKeyLong)
          {
            currentLongIndex = CurrentIndexCount;
            ((IInt64Key)entity).Id = currentLongIndex.KeyCount + 1;
          }
          else
          {
            var key = thisType.GetProperties().Where(b => b.Name == "Id").FirstOrDefault();
            if (key != null)
            {
              key.SetValue(entity, Guid.NewGuid().ToString());
            }
          }
          var type = entity.GetType();
          var ignoredKey = type.GetProperties().Where(b =>
              b.CustomAttributes.Any(a =>
                a.AttributeType.Name.Contains("NotMappedAttribute") || a.AttributeType.Name.Contains("IgnoreEdit"))
            ).Select(b => b.Name).ToList();
          var bson = entity.ToBsonDocument<T>();
          ignoredKey.ForEach(b => bson.Remove(b));
          collectionBson.InsertOne(bson);
          if (currentLongIndex != null)
          {
            currentLongIndex.KeyCount = currentLongIndex.KeyCount + 1;
            AddCurrentIndex(currentLongIndex);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      };
      this.repo.ActionList.Add(action);
      return entity;
    }

    public T Attach(T entity)
    {
      return Add(entity);
    }

    public T Create()
    {
      throw new NotImplementedException();
    }

    public T Find(params object[] keyValues)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      return thisQ.GetEnumerator();
    }

    public T Remove(T entity)
    {
      Action action = () =>
      {
        try
        {
          var id = entity.GetType().GetProperties().Where(b => b.Name == "Id").FirstOrDefault();
          if (id == null)
          {
            return;
          }
          var idValue = id.GetValue(entity);
          this.collection.DeleteOne(new { Id = idValue }.ToBsonDocument());
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      };
      this.repo.ActionList.Add(action);
      return entity;
    }

    public void Update(object entity)
    {
      var type = typeof(T);
      var idKey = type.GetProperties().FirstOrDefault(b => b.Name == "Id");
      if (idKey == null)
        return;
      var idValue = idKey.GetValue(entity);
      var tableName = this.table;
      var bson = entity.ToBsonDocument();
      bson.Remove("_id");
      var ignoredKey = type.GetProperties().Where(b =>
        b.CustomAttributes.Any(a =>
          a.AttributeType.Name.Contains("NotMappedAttribute") || a.AttributeType.Name.Contains("IgnoreEdit"))
      ).Select(b => b.Name).ToList();
      ignoredKey.ForEach(b => bson.Remove(b));
      Action action = () =>
      {
        this.collection.UpdateOne(new { Id = idValue }.ToBsonDocument(), new BsonDocument { { "$set", bson } });
      };
      this.repo.ActionList.Add(action);
    }

    TDerivedEntity IDbSet<T>.Create<TDerivedEntity>()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return thisQ.GetEnumerator();
    }
  }
}