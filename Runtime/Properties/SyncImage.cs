using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncImage : ISyncProperty
    {
        private Image image;
        private Color oColor;
        private string spriteName;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var spritePath = vmPropertyValue.ToString();

            image = uiComponent as Image;
            oColor = image.color;
            spriteName = Path.GetFileName(spritePath);

            image.color = Color.clear;

            bindingContext.LoadAssetAsync<Sprite>(spritePath, sprite =>
            {
                if (image == null)
                    return;

                if (sprite != null && sprite.name.Equals(Path.GetFileNameWithoutExtension(spriteName), System.StringComparison.Ordinal))
                {
                    image.sprite = sprite;
                }
                image.color = oColor;
            });
        }
    }
}
