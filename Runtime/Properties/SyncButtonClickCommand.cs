using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncButtonClickCommand : ISyncProperty
    {
        private ICommand command;
        private BindingContext bindingContext;
        private bool listened;
        private RectTransform rectTransform;
        private ButtonClickCommandArgs clickArgs;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            this.bindingContext = bindingContext;
            command = vmPropertyValue as SimpleCommand;
            rectTransform = uiComponent.GetComponent<RectTransform>();
            clickArgs = new ButtonClickCommandArgs();

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
            var listViewArgs = bindingData as ListViewCommandArgs;
            clickArgs.transform = new Transform(rectTransform);
            clickArgs.listViewArgs = listViewArgs;
            command.Execute(clickArgs);
        }
    }
}
