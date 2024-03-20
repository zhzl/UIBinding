using UnityEngine;

namespace UIBinding
{
    public class SyncAlpha : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var canvasGroup = uiComponent as CanvasGroup;
            var alpha = (vmPropertyValue as Boxed<float>).value;
            canvasGroup.alpha = alpha;
        }
    }
}
