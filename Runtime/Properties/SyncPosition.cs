using UnityEngine;

namespace UIBinding
{
    public class SyncPosition : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var rectTransform = uiComponent as RectTransform;
            if (vmPropertyValue is Boxed<Vector2> pos2)
            {
                rectTransform.localPosition = pos2.value;
            }
            else if (vmPropertyValue is Boxed<Vector3> pos3)
            {
                rectTransform.localPosition = pos3.value;
            }
        }
    }
}
