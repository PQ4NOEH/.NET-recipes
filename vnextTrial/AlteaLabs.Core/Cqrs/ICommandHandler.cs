namespace AlteaLabs.Core.Cqrs
{
    public interface ICommandHandler<T> where T : ICommand
    {
        ICommandHandlerResult Handler(T command);
    }
}
