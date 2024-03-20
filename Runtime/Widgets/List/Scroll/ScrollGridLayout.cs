using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 网格布局
    /// </summary>
    public class ScrollGridLayout : ScrollLayoutBase
    {
        public float headPadding;
        public float tailPadding;
        public int groupCellCount = 1;
        public Vector2 spacing;
        public Vector2 cellSize;

        public override float CalcContentHeight(int cellCount)
        {
            var groupCount = (cellCount + groupCellCount - 1) / groupCellCount;
            var groupHeight = direction == Direction.Horizontal ? cellSize.x : cellSize.y;
            var groupSpacing = direction == Direction.Horizontal ? spacing.x : spacing.y;
            return headPadding + tailPadding + (groupHeight + groupSpacing) * groupCount - groupSpacing;
        }

        public override Vector2 CalcCellPosition(RectTransform cellRect, int cellIndex)
        {
            var contentHeight = CalcContentHeight(scrollView.CellCount);
            var halfContentHeight = contentHeight * 0.5f;

            var groupHeight = direction == Direction.Horizontal ? cellSize.x : cellSize.y;
            var groupSpacing = direction == Direction.Horizontal ? spacing.x : spacing.y;
            var groupPivot = direction == Direction.Horizontal ? cellRect.pivot.x : cellRect.pivot.y;
            var groupIndex = cellIndex / groupCellCount;
            var offset = halfContentHeight - (1 - groupPivot) * groupHeight - headPadding - (groupHeight + groupSpacing) * groupIndex;

            var innerHeight = direction == Direction.Horizontal ? cellSize.y : cellSize.x;
            var innerSpacing = direction == Direction.Horizontal ? spacing.y : spacing.x;
            var innerPivot = direction == Direction.Horizontal ? cellRect.pivot.y : cellRect.pivot.x;
            var innerIndex = cellIndex - groupIndex * groupCellCount;
            var innerOffset = innerPivot * innerHeight + (innerHeight + innerSpacing) * innerIndex;
            innerOffset -= ((innerHeight + innerSpacing) * groupCellCount - innerSpacing) * 0.5f;
            return direction == Direction.Horizontal ? new Vector2(-offset, -innerOffset) : new Vector2(innerOffset, offset);
        }

        public override void CalcDisplayIndexes(List<int> displayIndexes)
        {
            displayIndexes.Clear();

            var contentOffset = direction == Direction.Horizontal ? -scrollRect.content.anchoredPosition.x : scrollRect.content.anchoredPosition.y;
            var viewportHeight = direction == Direction.Horizontal ? scrollRect.viewport.rect.width : scrollRect.viewport.rect.height;

            var groupHeight = direction == Direction.Horizontal ? cellSize.x : cellSize.y;
            var groupSpacing = direction == Direction.Horizontal ? spacing.x : spacing.y;

            var startGroupIndex = (int)((contentOffset - headPadding + groupSpacing) / (groupHeight + groupSpacing));
            var endGroupIndex = startGroupIndex + 1 + (int)((viewportHeight + groupSpacing) / (groupHeight + groupSpacing));
            for (int i = startGroupIndex; i <= endGroupIndex; i++)
            {
                for (int j = 0; j < groupCellCount; j++)
                {
                    var cellIndex = i * groupCellCount + j;

                    if (cellIndex < 0 || cellIndex >= scrollView.CellCount)
                        continue;

                    displayIndexes.Add(cellIndex);
                }
            }
        }

        protected override Vector2 CalcEndLocalPosition(int cellIndex)
        {
            var groupHeight = direction == Direction.Horizontal ? cellSize.x : cellSize.y;
            var groupSpacing = direction == Direction.Horizontal ? spacing.x : spacing.y;
            var groupIndex = cellIndex / groupCellCount;
            var contentHeight = CalcContentHeight(scrollView.CellCount);
            var viewportHeight = direction == Direction.Horizontal ? scrollRect.viewport.rect.width : scrollRect.viewport.rect.height;
            var offset = headPadding + (groupHeight + groupSpacing) * groupIndex;
            var minOffset = 0;
            var maxOffset = contentHeight - viewportHeight;
            offset = Mathf.Max(minOffset, offset);
            offset = Mathf.Min(maxOffset, offset);
            return direction == Direction.Horizontal ? new Vector2(-offset, 0) : new Vector2(0, offset);
        }
    }
}
