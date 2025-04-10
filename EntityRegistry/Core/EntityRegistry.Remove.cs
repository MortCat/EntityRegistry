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
        public virtual bool Remove(string entityName)
        {
            return RemoveInternal(entityName);
        }
        protected virtual bool RemoveInternal(string entityName)
        {
            if (_items.TryRemove(entityName, out var entity))
            {
                RaiseRemovedSuccess(entity);
                return true;
            }
            else
            {
                RaiseRemovedFailed(default);
                return false;
            }
        }
    }
}
