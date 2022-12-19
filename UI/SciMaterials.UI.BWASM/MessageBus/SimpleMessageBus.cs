using System.Collections.Concurrent;
using SciMaterials.UI.BWASM.MessageBus;

public delegate void MessageHandler<TMessageArgs>(object? sender, TMessageArgs message) where TMessageArgs : IBusMessage;

public class SimpleMessageBus : ISimpleMessageBus
{
    private const int m_triesCount = 10;

    private readonly ConcurrentDictionary<Type, Delegate> _subscriptions = new();

    public void Subscribe<T>(MessageHandler<T> handler) where T : IBusMessage
    {
        if (handler is null)
            throw new NullReferenceException(nameof(handler));

        Type type = typeof(T);
        _subscriptions.AddOrUpdate(type, handler, (key, value) =>
        {
            if (value is MessageHandler<T> existedHandler)
            {
                existedHandler += handler;
                return existedHandler;
            }

            return value;
        });
    }

    public void Unsubscribe<T>(MessageHandler<T> handler) where T : IBusMessage
    {
        if (handler is null)
            throw new NullReferenceException(nameof(handler));

        Type type = typeof(T);
        int tries = 0;
        while (_subscriptions.TryGetValue(type, out var existedDelegate) && tries < m_triesCount)
        {
            if (existedDelegate is MessageHandler<T> newHandler)
            {
                newHandler -= handler;
                {
                    if (_subscriptions.TryUpdate(type, newHandler, existedDelegate))
                        return;
                }
            }
            tries++;
        }
    }

    public void Publish<T>(object? sender, T message) where T : IBusMessage
    {
        if (message is null)
            throw new NullReferenceException(nameof(message));

        if (_subscriptions.TryGetValue(typeof(T), out Delegate? handler))
        {
            (handler as MessageHandler<T>)?.Invoke(sender, message);
        }
    }
}