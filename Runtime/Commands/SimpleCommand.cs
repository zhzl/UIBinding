namespace UIBinding
{
    public class SimpleCommand : ICommand
    {
        private readonly CommandAction commandAction;

        public SimpleCommand(CommandAction commandAction)
        {
            this.commandAction = commandAction;
        }

        public void Execute(ICommandArgs args)
        {
            this.commandAction?.Invoke(args);
        }
    }
}
