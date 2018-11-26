using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Persistence
{
    public interface IEntity<EntityType, Key, SCriteria>
    {
        void Create(EntityType item);
        void Delete(Key key);
        void Delete(EntityType item);
        IEnumerable<EntityType> Search(SCriteria searchCriteria);
        IEnumerable<EntityType> Search(object state, SCriteria searchCriteria);
        void Update(EntityType item);
    }

    public abstract class EntityBase<EntityType, Key, SCriteria, DataContext>
        where DataContext : IDisposable
    {
        #region Constructors

        public EntityBase(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        #endregion

        #region Properties

        public bool IsReadOnly
        {
            get;
            private set;
        }

        #endregion

        public virtual void Create(EntityType item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("The entity is read-only!");

            DoOperation((dc) =>
            {
                CreateInternal(dc, item);
            });
        }

        public virtual EntityType Read(Key key)
        {
            return DoOperation<EntityType>((dc) =>
            {
                return ReadInternal(dc, key);
            });
        }

        public virtual void Update(EntityType item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("The entity is read-only!");

            DoOperation((dc) =>
            {
                UpdateInternal(dc, item);
            });
        }

        public virtual void Delete(Key key)
        {
            if (IsReadOnly)
                throw new NotSupportedException("The entity is read-only!");

            DoOperation((dc) =>
            {
                DeleteInternal(dc, key);
            });
        }

        public virtual void Delete(EntityType item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("The entity is read-only!");

            DoOperation((dc) =>
            {
                DeleteInternal(dc, item);
            });
        }

        public virtual IEnumerable<EntityType> Search(SCriteria searchCriteria)
        {
            return DoOperation<IEnumerable<EntityType>>((dc) =>
            {
                return SearchInternal(dc, searchCriteria);
            });
        }

        public virtual IEnumerable<EntityType> Search(object state, SCriteria searchCriteria)
        {
            return DoOperation<IEnumerable<EntityType>>((dc) =>
            {
                return SearchInternal(dc, state, searchCriteria);
            });
        }

        protected virtual void CreateInternal(DataContext context, EntityType item)
        {
            throw new NotImplementedException();
        }

        protected virtual EntityType ReadInternal(DataContext context, Key key)
        {
            throw new NotImplementedException();
        }

        protected virtual void UpdateInternal(DataContext context, EntityType item)
        {
            throw new NotImplementedException();
        }

        protected virtual void DeleteInternal(DataContext context, Key key)
        {
            throw new NotImplementedException();
        }

        protected virtual void DeleteInternal(DataContext context, EntityType item)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<EntityType> SearchInternal(DataContext context, SCriteria searchCriteria)
        {
            return SearchInternal(context, null, searchCriteria);
        }

        protected virtual IEnumerable<EntityType> SearchInternal(DataContext context, object state, SCriteria searchCriteria)
        {
            throw new NotImplementedException();
        }

        protected virtual DbContext CreateDbContext()
        {
            return REAUDbContext.CreateDefaultSerializationContext();
        }

        protected virtual DataContext CreateDataContext(DbContext context)
        {
            var res = (DataContext)Activator.CreateInstance(typeof(DataContext), new object[] { context.Connection });

            return res;
        }

        #region Helpers
        protected virtual void DoOperation(Action<DataContext> action)
        {
            using (DbContext context = CreateDbContext())
            {
                using (DataContext dc = CreateDataContext(context))
                {
                    action(dc);
                }

                context.Complete();
            }
        }

        protected virtual TResult DoOperation<TResult>(Func<DataContext, TResult> action)
        {
            TResult result = default(TResult);

            using (DbContext context = CreateDbContext())
            {
                using (DataContext dc = CreateDataContext(context))
                {
                    result = action(dc);
                }

                context.Complete();
            }

            return result;
        }
        #endregion
    }
}
