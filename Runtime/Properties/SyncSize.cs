using UnityEngine;

namespace UIBinding
{
    public class SyncSize : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var rectTransform = uiComponent as RectTransform;
            var size = (vmPropertyValue as Boxed<Vector2>).value;
            rectTransform.SetRectSize(size);
        }
    }
}
