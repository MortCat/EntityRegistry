using EntityRegistry.Event;
using System.Collections.Concurrent;
using System.Reflection;

namespace EntityRegistry.Core
{
    public interface IEntity<T> : ICloneableEntity<T>
    {
        //Guid Id { get; } // Refactored to module property.
        string Name { get; }
        //T entity { get; }
    }
    public interface ICloneableEntity<T>
    {
        T Clone();
    }

    public abstract partial class EntityRegistry<T> : IEventStream<T>  
        where T : IEntity<T> 
    {
        public event Action<T>? AddedSuccess;
        public event Action<T>? RemovedSuccess;
        public event Action<T>? UpdatedSuccess;

        public event Action<T>? AddedFailed;
        public event Action<T>? RemovedFailed;
        public event Action<T>? UpdatedFailed;
        public event Action<T>? GetFailed;

        private readonly ConcurrentDictionary<string, T> _items = new();


        public virtual bool Contains(string entityName) => _items.ContainsKey(entityName);
        public T? GetDeepClone(string entityName)
        {
            if (_items.TryGetValue(entityName, out var value))
            {
                return DeepClone(value);
            }
            else
            {
                RaiseGetFailed(default);
                return default;
            }
        }
        protected virtual T DeepClone(T entity)
        {
            return entity.Clone();
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


        protected void RaiseAddedSuccess(T entity) => AddedSuccess?.Invoke(entity);
        protected void RaiseAddedFailed(T entity) => AddedFailed?.Invoke(entity);
        protected void RaiseRemovedSuccess(T entity) => RemovedSuccess?.Invoke(entity);
        protected void RaiseRemovedFailed(T entity) => RemovedFailed?.Invoke(entity);
        protected void RaiseUpdatedSuccess(T entity) => UpdatedSuccess?.Invoke(entity);
        protected void RaiseUpdatedFailed(T entity) => UpdatedFailed?.Invoke(entity);
        protected void RaiseGetFailed(T entity) => GetFailed?.Invoke(entity);
    }
}
