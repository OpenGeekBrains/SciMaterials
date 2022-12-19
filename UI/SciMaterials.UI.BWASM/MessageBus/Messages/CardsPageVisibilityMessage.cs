using SciMaterials.UI.BWASM.MessageBus;

public record ResourceCardsPageVisibilityMessage(bool IsVisible) : IBusMessage;
