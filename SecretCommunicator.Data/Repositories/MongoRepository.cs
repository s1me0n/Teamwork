using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SecretCommunicator.Data.Interfaces;
using SecretCommunicator.Models.Interfaces;
using MongoDB.Driver.Builders;
using MongoDB.Bson;


namespace SecretCommunicator.Data.Repositories
{
    public class MongoRepository : IRepository
    {
        SessionState session = new SessionState();

        public IQueryable<T> All<T>(string[] includes = null) where T : class
        {
            return session.db.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public T Get<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression, string[] includes = null) where T : class
        {
            return All<T>(includes).FirstOrDefault(expression);
        }
        public IQueryable<T> Find<T>(string id) where T : class
        {
            return session.db.GetCollection<T>(typeof(T).Name).Find(Query.EQ("Id", id)).AsQueryable();
        }

        public T Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Filter<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Filter<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50, string[] includes = null) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Contains<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            throw new NotImplementedException();
        }

        public T Create<T>(T t) where T : class
        {
            var result = session.db.GetCollection<T>(typeof(T).Name).Save(t, SafeMode.True);
            return t;
        }

        public void Delete<T>(string id) where T : class
        {
            var result = session.db.GetCollection<T>(typeof(T).Name).Remove(Query.EQ("_id", id));
        }

        public int Update<T>(T t) where T : class
        {
            this.Create(t);
            return 1;
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void ExecuteProcedure(string procedureCommand, params System.Data.SqlClient.SqlParameter[] sqlParams)
        {
            throw new NotImplementedException();
        }

        
    }
}
