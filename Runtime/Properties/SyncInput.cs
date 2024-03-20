using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncInput : ISyncProperty
    {
        private BindingContext bindingContext;
        private bool listened;
        private string propertyName;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            this.bindingContext = bindingContext;
            this.propertyName = bindingContext.GetBindingData<string>(EBindingData.PropertyName);

            var input = uiComponent as InputField;
            var inputStr = vmPropertyValue.ToString();
            if (!input.text.Equals(inputStr, System.StringComparison.Ordinal))
            {
                input.SetTextWithoutNotify(inputStr);
            }

            if (!listened)
            {
                listened = true;
                input.onEndEdit.AddListener(OnInputEndEdit);
            }
        }

        private void OnInputEndEdit(string endStr)
        {
            bindingContext.UpdatePropertyValue(propertyName, endStr);
        }
    }
}
