using EntityRegistry.Core;
using EntityRegistry.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityRegistry.Single
{
    public abstract partial class SingleEntityRegistry<T>: IEventStream<T>
        where T : IEntity<T>
    {
        public event Action<T>? AddedSuccess;
        public event Action<T>? RemovedSuccess;
        public event Action<T>? UpdatedSuccess;
        public event Action<T>? AddedFailed;
        public event Action<T>? RemovedFailed;
        public event Action<T>? UpdatedFailed;
        public event Action<T>? GetFailed;
        private T? _item = default;

        public virtual bool HasEntity => _item != null;

        public virtual void Set(T entity)
        {
            _item = entity;
            RaiseAddedSuccess(entity);
        }
        public virtual void Clear(T entity)
        {
            var removed = _item;
            _item = default;
            RaiseRemovedSuccess(removed);
        }
        public virtual void Update(T entity)
        {
            if (_item != null)
            {
                CopyPropertiesFrom(entity, _item);
                RaiseUpdatedSuccess(_item);
            }
            else
            {
                RaiseUpdatedFailed(entity);
            }
        }

        private void CopyPropertiesFrom(T source, T target)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in props)
            {
                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }
        }
        public T? GetDeepClone()
        {
            if (_item != null)
                return DeepClone(_item);

            RaiseGetFailed(default!);
            return default;
        }

        protected virtual T DeepClone(T entity)
        {
            return entity.Clone();
        }

        protected void RaiseAddedSuccess(T entity) => AddedSuccess?.Invoke(entity);
        protected void RaiseAddedFailed(T entity) => AddedFailed?.Invoke(entity);
        protected void RaiseRemovedSuccess(T entity) => RemovedSuccess?.Invoke(entity);
        protected void RaiseRemovedFailed(T entity) => RemovedFailed?.Invoke(entity);
        protected void RaiseUpdatedSuccess(T entity) => UpdatedSuccess?.Invoke(entity);
        protected void RaiseUpdatedFailed(T entity) => UpdatedFailed?.Invoke(entity);
        protected void RaiseGetFailed(T entity) => GetFailed?.Invoke(entity);
    }
}
