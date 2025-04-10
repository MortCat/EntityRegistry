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
        public virtual bool Update(T entity)
        {
            return UpdateProperties(entity);
        }
        protected virtual bool UpdateProperties(T entity)
        {
            if (_items.TryGetValue(entity.Name, out var existing))
            {
                CopyPropertiesFrom(entity, existing);
                RaiseUpdatedSuccess(existing);
                return true;
            }

            RaiseUpdatedFailed(entity);
            return false;
        }
        public virtual bool Replace(T entity)
        {
            return ReplaceInternal(entity);
        }
        protected virtual bool ReplaceInternal(T entity)
        {
            if (_items.AddOrUpdate(entity.Name, entity, (_, __) => entity) != null)
            {
                RaiseUpdatedSuccess(entity);
                return true;
            }

            RaiseUpdatedFailed(entity);
            return false;
        }
    }
}
