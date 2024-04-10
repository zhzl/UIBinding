using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncButtonInteractable : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var button = uiComponent as Button;
            var interactable = (vmPropertyValue as Boxed<bool>).value;
            button.interactable = interactable;
        }
    }
}
