using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncImageFillAmount : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var image = uiComponent as Image;
            var fillRate = (vmPropertyValue as Boxed<float>).value;
            image.fillAmount = fillRate;
        }
    }
}
