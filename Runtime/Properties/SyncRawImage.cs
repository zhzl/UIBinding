using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class SyncRawImage : ISyncProperty
    {
        private RawImage image;
        private Color oColor;
        private string textureName;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var texturePath = vmPropertyValue.ToString();

            image = uiComponent as RawImage;
            oColor = image.color;
            textureName = Path.GetFileName(texturePath);

            image.color = Color.clear;

            bindingContext.LoadAssetAsync<Texture>(texturePath, texture =>
            {
                if (image == null)
                    return;

                if (texture != null && texture.name.Equals(textureName, System.StringComparison.Ordinal))
                {
                    image.texture = texture;
                }
                image.color = oColor;
            });
        }
    }
}
