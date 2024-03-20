using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 列表布局
    /// </summary>
    public class ScrollListLayout : ScrollLayoutBase
    {
        public float headPadding;
        public float tailPadding;
        public float spacing;
        public float cellHeight;

        public override float CalcContentHeight(int cellCount)
        {
            return headPadding + tailPadding + (cellHeight + spacing) * cellCount - spacing;
        }

        public override Vector2 CalcCellPosition(RectTransform cellRect, int cellIndex)
        {
            var contentHeight = CalcContentHeight(scrollView.CellCount);
            var halfContentHeight = contentHeight * 0.5f;
            var cellPivot = direction == Direction.Horizontal ? cellRect.pivot.x : cellRect.pivot.y;
            var offset = halfContentHeight - (1 - cellPivot) * cellHeight - headPadding - (cellHeight + spacing) * cellIndex;
            return direction == Direction.Horizontal ? new Vector2(-offset, 0) : new Vector2(0, offset);
        }

        public override void CalcDisplayIndexes(List<int> displayIndexes)
        {
            displayIndexes.Clear();

            var contentOffset = direction == Direction.Horizontal ? -scrollRect.content.anchoredPosition.x : scrollRect.content.anchoredPosition.y;
            var viewportHeight = direction == Direction.Horizontal ? scrollRect.viewport.rect.width : scrollRect.viewport.rect.height;

            var startIndex = (int)((contentOffset - headPadding + spacing) / (cellHeight + spacing));
            var endIndex = startIndex + 1 + (int)((viewportHeight + spacing) / (cellHeight + spacing));
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i < 0 || i >= scrollView.CellCount)
                    continue;

                displayIndexes.Add(i);
            }
        }

        protected override Vector2 CalcEndLocalPosition(int cellIndex)
        {
            var contentHeight = CalcContentHeight(scrollView.CellCount);
            var viewportHeight = direction == Direction.Horizontal ? scrollRect.viewport.rect.width : scrollRect.viewport.rect.height;
            var offset = headPadding + (cellHeight + spacing) * cellIndex;
            var minOffset = 0;
            var maxOffset = contentHeight - viewportHeight;
            offset = Mathf.Max(minOffset, offset);
            offset = Mathf.Min(maxOffset, offset);
            return direction == Direction.Horizontal ? new Vector2(-offset, 0) : new Vector2(0, offset);
        }
    }
}
