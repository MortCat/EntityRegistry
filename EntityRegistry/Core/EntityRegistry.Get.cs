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
        public T? Get(string entityName)
        {
            if (_items.TryGetValue(entityName, out var value))
            {
                return value;
            }
            else
            {
                RaiseGetFailed(default);
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
    }
}
