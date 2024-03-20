using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncToggleValue : ISyncProperty
    {
        private BindingContext bindingContext;
        private bool listened;
        private string propertyName;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            this.bindingContext = bindingContext;
            this.propertyName = bindingContext.GetBindingData<string>(EBindingData.PropertyName);

            var toggle = uiComponent as Toggle;
            var value = (vmPropertyValue as Boxed<bool>).value;
            if (value != toggle.isOn)
            {
                toggle.SetIsOnWithoutNotify(value);
            }

            if (!listened)
            {
                listened = true;
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        private void OnToggleValueChanged(bool value)
        {
            bindingContext.UpdatePropertyValue(propertyName, value);
        }
    }
}
