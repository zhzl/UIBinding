namespace UIBinding
{
    public class LoadPrefabCommand : ICommand
    {
        public string prefabPath;

        private readonly CommandAction commandAction;

        public LoadPrefabCommand(string prefabPath, CommandAction commandAction)
        {
            this.prefabPath = prefabPath;
            this.commandAction = commandAction;
        }

        public void Execute(ICommandArgs args)
        {
            this.commandAction?.Invoke(args);
        }
    }
}
