using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncImageAlpha : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var image = uiComponent as Image;
            var alpha = (vmPropertyValue as Boxed<float>).value;
            var c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
}
