using UnityEngine;

namespace UIBinding
{
    public class SyncScale : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var rectTransform = uiComponent as RectTransform;
            if (vmPropertyValue is Boxed<Vector2> pos2)
            {
                rectTransform.localScale = pos2.value;
            }
            else if (vmPropertyValue is Boxed<Vector3> pos3)
            {
                rectTransform.localScale = pos3.value;
            }
        }
    }
}
