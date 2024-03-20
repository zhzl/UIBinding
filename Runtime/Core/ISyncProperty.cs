using UnityEngine;

namespace UIBinding
{
    public interface ISyncProperty
    {
        void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext);
    }
}
