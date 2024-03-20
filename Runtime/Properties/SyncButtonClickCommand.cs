using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncButtonClickCommand : ISyncProperty
    {
        private ICommand command;
        private BindingContext bindingContext;
        private bool listened;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            this.bindingContext = bindingContext;
            command = vmPropertyValue as SimpleCommand;

            if (!listened)
            {
                listened = true;
                var button = uiComponent as Button;
                button.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            var bindingData = bindingContext.GetBindingData(EBindingData.ListViewCommandArgs);
            var commandArgs = bindingData as ICommandArgs;
            command.Execute(commandArgs);
        }
    }
}
