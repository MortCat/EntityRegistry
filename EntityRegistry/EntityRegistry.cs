﻿using System.Collections.Concurrent;

namespace EntityRegistry
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
    public abstract class EntityRegistry<T> where T : IEntity<T>
    {
        public event Action<T>? AddedSuccess;
        public event Action<T>? RemovedSuccess;
        public event Action<T>? UpdatedSuccess;

        public event Action<T>? AddedFailed;
        public event Action<T>? RemovedFailed;
        public event Action<T>? UpdatedFailed;
        public event Action<T>? GetFailed;

        protected readonly ConcurrentDictionary<string, T> _items = new();

        public virtual bool Add(T entity)
        {
            return AddInternal(entity);
        }
        protected virtual bool AddInternal(T entity)
        {
            if (_items.TryAdd(entity.Name, entity))
            {
                AddedSuccess?.Invoke(entity);
                return true;
            }
            else
            {
                AddedFailed?.Invoke(entity);
                return false;
            }
        }
        public virtual bool Remove(string entityName)
        {
            return RemoveInternal(entityName);
        }
        protected virtual bool RemoveInternal(string entityName)
        {
            if (_items.TryRemove(entityName, out var entity))
            {
                RemovedSuccess?.Invoke(entity);
                return true;
            }
            else
            {
                RemovedFailed?.Invoke(default);
                return false;
            }
        }
        public virtual bool Update(T entity)
        {
            return UpdateInternal(entity);
        }
        protected virtual bool UpdateInternal(T entity)
        {
            if (_items.AddOrUpdate(entity.Name, entity, (_, __) => entity) != null)
            {
                UpdatedSuccess?.Invoke(entity);
                return true;
            }
            else
            {
                UpdatedFailed?.Invoke(entity);
                return false;
            }
        }
        public virtual bool Contains(string entityName) => _items.ContainsKey(entityName);

        public T? Get(string entityName)
        {
            if (_items.TryGetValue(entityName, out var value))
            {
                return value;
            }
            else
            {
                GetFailed?.Invoke(default);
                return default;
            }
        }
        public T? GetDeepClone(string entityName)
        {
            if (_items.TryGetValue(entityName, out var value))
            {
                return DeepClone(value);
            }
            else
            {
                GetFailed?.Invoke(default);
                return default;
            }
        }
        public IEnumerable<T> GetAll()
        {
            return _items.Values;
        }
        public IEnumerable<T> GetAll(Func<T, bool> predicate)
        {
            return GetAll().Where(predicate);
        }
        public IReadOnlyCollection<T> GetAllReadOnly()
        {
            return GetAll().ToList().AsReadOnly();
        }
        public IReadOnlyCollection<T> GetAllReadOnly(Func<T, bool> predicate)
        {
            return GetAll(predicate).ToList().AsReadOnly();
        }
        public IEnumerable<T> GetAllDeepClone()
        {
            return GetAll().Select(entity => DeepClone(entity));
        }
        public IEnumerable<T> GetAllDeepClone(Func<T, bool> predicate)
        {
            return GetAll().Where(predicate).Select(entity => DeepClone(entity));
        }
        /// <summary>
        /// 取得已啟用的資料，依速度排序，跳過 20 筆後取 10 筆
        /// var resultSample = manager.GetPaged(x => x.Flag, x => x.Speed, 20, 10);
        /// </summary>
        public IEnumerable<T> GetPaged(Func<T, bool> predicate, Func<T, object> orderBy, int skip, int take)
        {
            return GetAll(predicate)
                         .OrderBy(orderBy)
                         .Skip(skip)
                         .Take(take);
        }
        /// <summary>
        /// 單純排序 + 分頁
        /// var resultSample = manager.GetPaged(x => x.Name, 0, 50);
        /// </summary>
        public IEnumerable<T> GetPaged(Func<T, object> orderBy, int skip, int take)
        {
            return GetAll().OrderBy(orderBy)
                           .Skip(skip)
                           .Take(take);
        }
        protected virtual T DeepClone(T entity)
        {
            return (T)entity.Clone();
        }

        //public T? Get(Guid id) => _items.TryGetValue(id, out var value) ? value : default;

        //public bool Contains(Guid id) => _items.ContainsKey(id);
    }
}
