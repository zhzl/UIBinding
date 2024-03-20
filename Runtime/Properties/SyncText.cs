using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncText : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var text = uiComponent as Text;
            text.text = vmPropertyValue.ToString();
        }
    }
}
