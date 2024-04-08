using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 使用本框架时，不希望将Unity的Transform直接暴露给用户，所以这里做个封装
    /// </summary>
    public struct UITransform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Rect rect;
        public Vector2 pivot;
        public Vector2 anchorMin;
        public Vector2 anchorMax;

        public UITransform(RectTransform rectTransform)
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
