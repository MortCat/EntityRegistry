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
    }
}
