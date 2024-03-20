using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    /// <summary>
    /// 列表布局基类
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollLayoutBase : MonoBehaviour
    {
        /// <summary>
        /// 滚动方向
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// 水平
            /// </summary>
            Horizontal,
            /// <summary>
            /// 竖直
            /// </summary>
            Vertical
        }

        /// <summary>
        /// 关联的列表
        /// </summary>
        public RecycleScrollView scrollView;

        /// <summary>
        /// 关联的 ScrollRect
        /// </summary>
        public ScrollRect scrollRect;

        /// <summary>
        /// 滚动方向
        /// </summary>
        public Direction direction;

        /// <summary>
        /// 根据布局参数计算 Content 高度
        /// </summary>
        public abstract float CalcContentHeight(int cellCount);

        /// <summary>
        /// 获取指定下标的 Cell 位置
        /// </summary>
        public abstract Vector2 CalcCellPosition(RectTransform cellRect, int cellIndex);

        /// <summary>
        /// 计算当前显示的 cell indexes
        /// </summary>
        public abstract void CalcDisplayIndexes(List<int> displayIndexes);

        /// <summary>
        /// 重置 content size
        /// </summary>
        public void ResizeContent(int cellCount)
        {
            var contentHeight = CalcContentHeight(cellCount);
            if (direction == Direction.Horizontal)
            {
                scrollRect.content.SetRectWidth(contentHeight);
            }
            else
            {
                scrollRect.content.SetRectHeight(contentHeight);
            }
        }

        /// <summary>
        /// 更新 cell 位置
        /// </summary>
        public void UpdateCellPosition(RectTransform cellRect, int cellIndex)
        {
            var cellPosition = CalcCellPosition(cellRect, cellIndex);
            cellRect.SetAnchorPosition(cellPosition);
        }

        /// <summary>
        /// 滚动到指定下标的cell
        /// </summary>
        public void DoScroll(int cellIndex, float duration) => StartCoroutine(ScrollTo(CalcEndLocalPosition(cellIndex), duration));

        /// <summary>
        /// 跳转到指定下标的cell
        /// </summary>
        public void DoJump(int cellIndex) => scrollRect.content.localPosition = CalcEndLocalPosition(cellIndex);

        /// <summary>
        /// 停止滚动
        /// </summary>
        public bool StopScroll { get; set; }

        /// <summary>
        /// 计算跳转到指定cell的终点位置
        /// </summary>
        protected abstract Vector2 CalcEndLocalPosition(int cellIndex);

        /// <summary>
        /// 滚动到指定位置
        /// </summary>
        protected IEnumerator ScrollTo(Vector2 endPos, float duration)
        {
            StopScroll = false;
            scrollRect.velocity = Vector2.zero;
            var startPos = scrollRect.content.localPosition;
            var startTime = Time.unscaledTime;
            var endTime = startTime + duration;
            while (Time.unscaledTime < endTime)
            {
                if (StopScroll) yield break;
                var v = Evaluate(Time.unscaledTime - startTime, duration);
                var p = Vector3.LerpUnclamped(startPos, endPos, v);
                scrollRect.content.localPosition = p;
                yield return null;
            }
            scrollRect.content.localPosition = endPos;
        }

        /// <summary>
        /// 滚动函数：OutCubic
        /// </summary>
        public static float Evaluate(float elapsed, float duration)
        {
            var time = elapsed / duration;
            return 1 - (time = 1 - time) * time * time;
        }

        /// <summary>
        /// 对象销毁时调用
        /// </summary>
        private void OnDestroy()
        {
            StopScroll = true;
        }

#if UNITY_EDITOR

        int editorCellCount = 0;
        float editorContentHeight = 0f;
        bool inited = false;
        readonly List<RecycleScrollCell> cells = new List<RecycleScrollCell>();

        void Update()
        {
            if (Application.isPlaying)
                return;

            if (scrollView == null || scrollRect == null)
                return;

            var viewport = scrollRect.viewport;
            if (viewport.rect.width == 0 && viewport.rect.height == 0)
                return;

            var cellCount = FindAllCells();
            var contentHeight = CalcContentHeight(cellCount);
            if (!inited)
            {
                editorCellCount = cellCount;
                editorContentHeight = contentHeight;
                inited = true;
                return;
            }

            var directionChanged = IsDirectionChanged();
            if (editorCellCount == cellCount && editorContentHeight == contentHeight && !directionChanged)
                return;

            editorCellCount = cellCount;
            editorContentHeight = contentHeight;

            if (editorCellCount == 0)
                return;

            var newSize = direction == Direction.Horizontal ?
                          new Vector2(contentHeight, viewport.rect.height) :
                          new Vector2(viewport.rect.width, contentHeight);
            scrollRect.content.SetRectSize(newSize);

            if (directionChanged)
            {
                scrollRect.horizontal = direction == Direction.Horizontal;
                scrollRect.vertical = direction == Direction.Vertical;
                UnityEditor.EditorUtility.SetDirty(scrollRect);
            }

            UpdateCellCount();
            for (int i = 0; i < cells.Count; i++)
            {
                var cellRect = cells[i].RectTransform;
                var cellPosition = CalcCellPosition(cellRect, i);
                cellRect.SetAnchorPosition(cellPosition);
            }
        }

        private void UpdateCellCount()
        {
            var countField = scrollView.GetType().GetField("cellCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            countField.SetValue(scrollView, editorCellCount);
        }

        private bool IsDirectionChanged()
        {
            if (direction == Direction.Horizontal)
            {
                return !scrollRect.horizontal;
            }
            else
            {
                return !scrollRect.vertical;
            }
        }

        private int FindAllCells()
        {
            cells.Clear();
            var contentTransform = scrollRect.content.transform;
            for (int i = 0; i < contentTransform.childCount; i++)
            {
                var child = contentTransform.GetChild(i);
                var cell = child.GetComponent<RecycleScrollCell>();
                if (cell == null)
                    continue;

                cells.Add(cell);
            }

            return cells.Count;
        }

#endif
    }
}
