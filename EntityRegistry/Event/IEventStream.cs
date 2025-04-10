namespace EntityRegistry.Event
{
    public interface IEventStream<T>
    {
        event Action<T>? AddedSuccess;
        event Action<T>? RemovedSuccess;
        event Action<T>? UpdatedSuccess;

        event Action<T>? AddedFailed;
        event Action<T>? RemovedFailed;
        event Action<T>? UpdatedFailed;
        event Action<T>? GetFailed;
    }
}
