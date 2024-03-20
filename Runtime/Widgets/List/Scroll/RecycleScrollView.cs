using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIBinding
{
    public class RecycleScrollView : MonoBehaviour, IListView, IScrollView
    {
        private bool inited;
        private bool delayed;
        private ScrollRect scrollRect;
        private GameObject cellTemplate;
        private Action<int> cellUpdateHandler;
        private List<RecycleScrollCell> displayCells;
        private List<RecycleScrollCell> pooledCells;
        private int cellCount;
        private ScrollLayoutBase layout;
        private List<int> displayIndexes;
        private Vector2 lastPosition;

        private void OnScroll(Vector2 offset)
        {
            var currPosition = scrollRect.content.anchoredPosition;
            if (Mathf.Abs(currPosition.x - lastPosition.x) > 1f || Mathf.Abs(currPosition.y - lastPosition.y) > 1f)
            {
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample($"TryRefreshCells");
#endif
                TryRefreshCells();
#if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
#endif

                lastPosition = currPosition;
            }
        }

        private IEnumerator DelayRefresh()
        {
            yield return null;
            Refresh(cellCount);
        }

        private void TryRefreshCells(bool forceUpdateCell = false)
        {
            layout.CalcDisplayIndexes(displayIndexes);

            // cell 无变化
            if (displayCells.Count > 0 && displayCells.Count == displayIndexes.Count && displayCells[0].index == displayIndexes[0])
            {
                if (!forceUpdateCell) return;
            }

            for (int i = 0; i < displayCells.Count; i++)
            {
                var cell = displayCells[i];
                // 不可见，需要回收
                if (displayIndexes.IndexOf(cell.index) < 0)
                {
                    cell.SetVisible(false);
                    pooledCells.Add(cell);
                    displayCells.Remove(cell);
                    i--;
                }
                // 已存在指定下标的可见cell
                else
                {
                    // 刷新位置
                    layout.UpdateCellPosition(cell.RectTransform, cell.index);
                    displayIndexes.Remove(cell.index);
                }
            }

            bool insertHead = false;
            if (displayCells.Count > 0 && displayIndexes.Count > 0 && displayIndexes[0] < displayCells[0].index)
            {
                insertHead = true;
            }

            for (int i = 0; i < displayIndexes.Count; i++)
            {
                var cellIndex = displayIndexes[i];
                var cell = GetCellFromPool();
                cell.index = cellIndex;
                layout.UpdateCellPosition(cell.RectTransform, cellIndex);

                if (insertHead)
                {
                    // 更新cell层级，如果需要，可以做个开关
                    // cell.RectTransform.SetSiblingIndex(i);
                    displayCells.Insert(i, cell);
                }
                else
                {
                    // cell.RectTransform.SetSiblingIndex(displayCells.Count);
                    displayCells.Add(cell);
                }
            }

            if (forceUpdateCell)
            {
                // 所有 cell 都触发刷新
                for (int i = 0; i < displayCells.Count; i++)
                {
                    cellUpdateHandler?.Invoke(displayCells[i].index);
                }
            }
            else
            {
                // 只刷新最近出现的 cell
                for (int i = 0; i < displayIndexes.Count; i++)
                {
                    cellUpdateHandler?.Invoke(displayIndexes[i]);
                }
            }
        }

        private void CheckIndex(int index)
        {
            if (displayCells.Count <= 0)
            {
                throw new ArgumentOutOfRangeException($"there is no cell visible");
            }

            var minIndex = displayCells[0].index;
            var maxIndex = displayCells[displayCells.Count - 1].index;
            if (index < minIndex || index > maxIndex)
            {
                throw new ArgumentOutOfRangeException($"index {index} is less than min display cell index {minIndex} or greater than max display cell index {maxIndex}");
            }
        }

        private void CheckDisplay()
        {
            if (displayCells.Count <= 0)
            {
                throw new ArgumentOutOfRangeException($"there is no cell visible");
            }
        }

        private void CheckInit()
        {
            if (!inited)
                Init();
        }

        private RecycleScrollCell GetCellFromPool()
        {
            if (pooledCells.Count <= 0)
            {
                var newCell = GameObject.Instantiate(cellTemplate, cellTemplate.transform.parent);
                var cell = newCell.GetComponent<RecycleScrollCell>();
                cell.SetVisible(true);
                return cell;
            }
            else
            {
                var lastIndex = pooledCells.Count - 1;
                var cell = pooledCells[lastIndex];
                pooledCells.RemoveAt(lastIndex);
                cell.SetVisible(true);
                return cell;
            }
        }

        public int CellCount => cellCount;

        public void AddCellUpdateListener(Action<int> cellUpdateCallback)
        {
            cellUpdateHandler = cellUpdateCallback;
        }

        public GameObject GetCell(int index)
        {
            CheckIndex(index);
            var startIndex = displayCells[0].index;
            return displayCells[index - startIndex].gameObject;
        }

        public void Init()
        {
            if (inited)
                return;

            inited = true;

            displayCells = new List<RecycleScrollCell>();
            pooledCells = new List<RecycleScrollCell>();
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnScroll);

            var content = scrollRect.content;
            var childCount = content.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = content.transform.GetChild(i);
                var cell = child.GetComponent<RecycleScrollCell>();
                if (cell == null)
                    continue;

                if (cellTemplate == null)
                {
                    cellTemplate = cell.gameObject;
                }

                cell.SetVisible(false);
                pooledCells.Add(cell);
            }

            layout = GetComponent<ScrollLayoutBase>();
            displayIndexes = new List<int>();
            delayed = false;
            lastPosition = content.anchoredPosition;
        }

        public void Refresh(int count)
        {
            CheckInit();
            cellCount = count;

            // SCROLLRECT VIEWPORT RECT DIMENSIONS ARE 0 IN THE FIRST FRAME IF SCROLL BAR VISIBILITY IS SET TO "AUTO HIDE AND EXPAND VIEWPORT"
            if (!delayed && scrollRect.viewport.rect.width == 0 && scrollRect.viewport.rect.height == 0)
            {
                StartCoroutine(DelayRefresh());
                delayed = true;
                return;
            }

            layout.ResizeContent(cellCount);

#if UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample($"TryRefreshCells");
#endif
            TryRefreshCells(true);
#if UNITY_EDITOR
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        public void RefreshIndex(int index)
        {
            CheckIndex(index);
            cellUpdateHandler?.Invoke(index);
        }

        public void JumpToIndex(int index)
        {
            CheckDisplay();
            layout.DoJump(index);
        }

        public void ScrollToIndex(int index, float duration)
        {
            CheckDisplay();
            layout.DoScroll(index, duration);
        }

        public void JumpToHead()
        {
            CheckDisplay();
            layout.DoJump(0);
        }

        public void JumpToTail()
        {
            CheckDisplay();
            layout.DoJump(cellCount - 1);
        }

        public void ScrollToHead(float duration)
        {
            CheckDisplay();
            layout.DoScroll(0, duration);
        }

        public void ScrollToTail(float duration)
        {
            CheckDisplay();
            layout.DoScroll(cellCount - 1, duration);
        }

        public void SetScrollEnabled(bool enabled)
        {
            scrollRect.enabled = enabled;
            scrollRect.velocity = Vector2.zero;
            layout.StopScroll = !enabled;
        }
    }
}
