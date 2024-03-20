using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncSliderValue : ISyncProperty
    {
        private BindingContext bindingContext;
        private bool listened;
        private string propertyName;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            this.bindingContext = bindingContext;
            this.propertyName = bindingContext.GetBindingData<string>(EBindingData.PropertyName);

            var slider = uiComponent as Slider;
            var value = (vmPropertyValue as Boxed<float>).value;
            if (!Mathf.Approximately(slider.value, value))
            {
                slider.SetValueWithoutNotify(value);
            }

            if (!listened)
            {
                listened = true;
                slider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        private void OnSliderValueChanged(float value)
        {
            bindingContext.UpdatePropertyValue(propertyName, value);
        }
    }
}
