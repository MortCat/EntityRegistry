using EntityRegistry.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityRegistry.Core
{
    public abstract partial class EntityRegistry<T> : IEventStream<T>
        where T : IEntity<T>
    {
        public virtual bool Add(T entity)
        {
            return AddInternal(entity.Name, entity);
        }
        public virtual bool Add(string entityName, T entity)
        {
            return AddInternal(entityName, entity);
        }
        protected virtual bool AddInternal(string entityName, T entity)
        {
            if (_items.TryAdd(entityName, entity))
            {
                RaiseAddedSuccess(entity);
                return true;
            }
            else
            {
                RaiseAddedFailed(entity);
                return false;
            }
        }
        public virtual bool AddOrUpdate(T entity)
        {
            var existing = _items.GetOrAdd(entity.Name, entity);

            if (!ReferenceEquals(existing, entity))
            {
                CopyPropertiesFrom(entity, existing);
                RaiseUpdatedSuccess(existing);
            }
            else
            {
                RaiseAddedSuccess(entity);
            }

            return true;
        }
    }
}
