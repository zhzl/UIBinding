using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// RectTransform 相关扩展
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// 设置锚点位置
        /// </summary>
        /// <param name="newPos">新的锚点位置</param>
        /// <param name="epsilon">如果坐标差值 <= epsilon，则不更新</param>
        public static void SetAnchorPosition(this RectTransform rectTransform, Vector2 newPos, float epsilon = 0.01f)
        {
            var oldPos = rectTransform.anchoredPosition;
            if (Mathf.Abs(newPos.x - oldPos.x) > epsilon || Mathf.Abs(newPos.y - oldPos.y) > epsilon)
            {
                rectTransform.anchoredPosition = newPos;
            }
        }

        /// <summary>
        /// 设置锚点X位置
        /// </summary>
        /// <param name="newPos">新的锚点X位置</param>
        /// <param name="epsilon">如果坐标X差值 <= epsilon，则不更新</param>
        public static void SetAnchorPositionX(this RectTransform rectTransform, float x, float epsilon = 0.01f)
        {
            if (Mathf.Abs(x - rectTransform.anchoredPosition.x) > epsilon)
            {
                var newPos = rectTransform.anchoredPosition;
                newPos.x = x;
                rectTransform.anchoredPosition = newPos;
            }
        }

        /// <summary>
        /// 设置锚点Y位置
        /// </summary>
        /// <param name="newPos">新的锚点Y位置</param>
        /// <param name="epsilon">如果坐标Y差值 <= epsilon，则不更新</param>
        public static void SetAnchorPositionY(this RectTransform rectTransform, float y, float epsilon = 0.01f)
        {
            if (Mathf.Abs(y - rectTransform.anchoredPosition.y) > epsilon)
            {
                var newPos = rectTransform.anchoredPosition;
                newPos.y = y;
                rectTransform.anchoredPosition = newPos;
            }
        }

        /// <summary>
        /// 设置矩形大小
        /// </summary>
        /// <param name="newPos">新的矩形大小</param>
        /// <param name="epsilon">如果宽高差值均 <= epsilon，则不更新</param>
        public static void SetRectSize(this RectTransform rectTransform, Vector2 newSize, float epsilon = 0.01f)
        {
            if (Mathf.Abs(newSize.x - rectTransform.rect.width) > epsilon || Mathf.Abs(newSize.y - rectTransform.rect.height) > epsilon)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
            }
        }

        /// <summary>
        /// 设置矩形宽度
        /// </summary>
        /// <param name="newPos">新的矩形宽度</param>
        /// <param name="epsilon">如果宽度差值 <= epsilon，则不更新</param>
        public static void SetRectWidth(this RectTransform rectTransform, float width, float epsilon = 0.01f)
        {
            if (Mathf.Abs(width - rectTransform.rect.width) > epsilon)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
        }

        /// <summary>
        /// 设置矩形高度
        /// </summary>
        /// <param name="newPos">新的矩形高度</param>
        /// <param name="epsilon">如果高度差值 <= epsilon，则不更新</param>
        public static void SetRectHeight(this RectTransform rectTransform, float height, float epsilon = 0.01f)
        {
            if (Mathf.Abs(height - rectTransform.rect.height) > epsilon)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }
}
