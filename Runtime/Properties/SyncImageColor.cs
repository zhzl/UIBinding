using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncImageColor : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var image = uiComponent as Image;
            var color = (vmPropertyValue as Boxed<Color>).value;
            image.color = color;
        }
    }
}
