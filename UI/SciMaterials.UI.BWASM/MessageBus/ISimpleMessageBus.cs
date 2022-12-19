using SciMaterials.UI.BWASM.MessageBus;

public interface ISimpleMessageBus
{
    void Publish<T>(object? sender, T arg) where T : IBusMessage;
    void Subscribe<T>(MessageHandler<T> handler) where T : IBusMessage;
    void Unsubscribe<T>(MessageHandler<T> handler) where T : IBusMessage;
}
