using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using SecretCommunicator.Data.Interfaces;

namespace SecretCommunicator.Data.Repositories
{
    public class EfRepository : IRepository
    {
        public readonly DbContext _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(MusicstoreContext context)
        {
            _context = context;

            ////SERIALIZE WILL FAIL WITH PROXIED ENTITIES
            //_context.Configuration.ProxyCreationEnabled = false;

            ////ENABLING COULD CAUSE ENDLESS LOOPS AND PERFORMANCE PROBLEMS
            //_context.Configuration.LazyLoadingEnabled = false;
        }

        public IQueryable<T> All<T>(string[] includes = null) where T : class
        {
            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = _context.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query.AsQueryable();
            }

            return _context.Set<T>().AsQueryable();
        }

        public T Get<T>(Expression<Func<T, bool>> expression, string[] includes = null) where T : class
        {
            return All<T>(includes).FirstOrDefault(expression);
        }

        public virtual T Find<T>(Expression<Func<T, bool>> predicate, string[] includes = null) where T : class
        {
            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = _context.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query.FirstOrDefault<T>(predicate);
            }

            return _context.Set<T>().FirstOrDefault<T>(predicate);
        }

        public virtual IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate, string[] includes = null)
            where T : class
        {
            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = _context.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query.Where<T>(predicate).AsQueryable<T>();
            }

            return _context.Set<T>().Where<T>(predicate).AsQueryable<T>();
        }

        public virtual IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate, out int total, int index = 0,
            int size = 50, string[] includes = null) where T : class
        {
            int skipCount = index*size;
            IQueryable<T> _resetSet;

            //HANDLE INCLUDES FOR ASSOCIATED OBJECTS IF APPLICABLE
            if (includes != null && includes.Count() > 0)
            {
                var query = _context.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                _resetSet = predicate != null ? query.Where<T>(predicate).AsQueryable() : query.AsQueryable();
            }
            else
            {
                _resetSet = predicate != null
                    ? _context.Set<T>().Where<T>(predicate).AsQueryable()
                    : _context.Set<T>().AsQueryable();
            }

            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public virtual T Create<T>(T TObject) where T : class
        {
            ////ADD CREATE DATE IF APPLICABLE
            //if (TObject is ICreatedOn)
            //{
            //    (TObject as ICreatedOn).CreatedOn = DateTime.UtcNow;
            //}

            ////ADD LAST MODIFIED DATE IF APPLICABLE
            //if (TObject is IModifiedOn)
            //{
            //    (TObject as IModifiedOn).ModifiedOn = DateTime.UtcNow;
            //}

            var newEntry = _context.Set<T>().Add(TObject);
            _context.SaveChanges();
            return newEntry;
        }

        public virtual int Delete<T>(T TObject) where T : class
        {
            _context.Set<T>().Remove(TObject);
            return _context.SaveChanges();
        }

        public virtual int Update<T>(T TObject) where T : class
        {
            ////ADD LAST MODIFIED DATE IF APPLICABLE
            //if (TObject is IModifiedOn)
            //{
            //    (TObject as IModifiedOn).ModifiedOn = DateTime.UtcNow;
            //}

            var entry = _context.Entry(TObject);
            _context.Set<T>().Attach(TObject);
            entry.State = EntityState.Modified;
            return _context.SaveChanges();
        }

        public virtual int Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var objects = Filter<T>(predicate);
            foreach (var obj in objects)
                _context.Set<T>().Remove(obj);
            return _context.SaveChanges();
        }

        public bool Contains<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _context.Set<T>().Count<T>(predicate) > 0;
        }

        public virtual void ExecuteProcedure(String procedureCommand, params SqlParameter[] sqlParams)
        {
            _context.Database.ExecuteSqlCommand(procedureCommand, sqlParams);
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        //public void Dispose()
        //{
        //    if (_context != null)
        //        _context.Dispose();
        //}
    }
}
