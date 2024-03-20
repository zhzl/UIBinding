using UnityEngine;

namespace UIBinding
{
    public class SyncVisible : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var gameObject = uiComponent.gameObject;
            var visible = (vmPropertyValue as Boxed<bool>).value;
            gameObject.SetActive(visible);
        }
    }
}
