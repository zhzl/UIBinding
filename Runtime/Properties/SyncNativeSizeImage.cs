using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncNativeSizeImage : ISyncProperty
    {
        private Image image;
        private string spriteName;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var spritePath = vmPropertyValue.ToString();

            image = uiComponent as Image;
            spriteName = Path.GetFileName(spritePath);

            bindingContext.LoadAssetAsync<Sprite>(spritePath, sprite =>
            {
                if (image == null)
                    return;

                if (sprite != null && sprite.name.Equals(Path.GetFileNameWithoutExtension(spriteName), System.StringComparison.Ordinal))
                {
                    image.sprite = sprite;
                    image.SetNativeSize();
                }
            });
        }
    }
}
