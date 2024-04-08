using UnityEngine;

namespace UIBinding
{
    public struct Transform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Rect rect;
        public Vector2 pivot;
        public Vector2 anchorMin;
        public Vector2 anchorMax;

        public Transform(RectTransform rectTransform)
        {
            position = rectTransform.position;
            rotation = rectTransform.eulerAngles;
            scale = rectTransform.localScale;
            rect = rectTransform.rect;
            pivot = rectTransform.pivot;
            anchorMin = rectTransform.anchorMin;
            anchorMax = rectTransform.anchorMax;
        }
    }
}
