using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 静态列表
    /// </summary>
    public class StaticListView : MonoBehaviour, IListView
    {
        /// <summary>
        /// 使用预制上的单元格下标
        /// </summary>
        public bool usePrefabIndex;

        private GameObject cellTemplate;
        private Action<int> cellUpdateHandler;
        private List<StaticListCell> cells;
        private int cellCount;

        public void AddCellUpdateListener(Action<int> cellUpdateCallback)
        {
            cellUpdateHandler = cellUpdateCallback;
        }

        public GameObject GetCell(int index)
        {
            CheckIndex(index);
            return cells[index].gameObject;
        }

        public void Init()
        {
            cells = new List<StaticListCell>();
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                var cellComp = child.GetComponent<StaticListCell>();
                if (cellComp == null)
                    continue;

                if (cellTemplate == null)
                {
                    cellTemplate = cellComp.gameObject;
                }

                cells.Add(cellComp);
            }
        }

        public void Refresh(int count)
        {
            cellCount = count;

            while (cells.Count < count)
            {
                var newCell = GameObject.Instantiate(cellTemplate, cellTemplate.transform.parent);
                var cellComp = newCell.GetComponent<StaticListCell>();
                cells.Add(cellComp);
            }

            // 刷新单元格
            for (int i = 0; i < count; i++)
            {
                if (!cells[i].gameObject.activeSelf)
                {
                    cells[i].gameObject.SetActive(true);
                }
                if (!usePrefabIndex)
                {
                    cells[i].index = i;
                }
                cellUpdateHandler?.Invoke(i);
            }

            for (int i = count; i < cells.Count; i++)
            {
                cells[i].gameObject.SetActive(false);
            }
        }

        public void RefreshIndex(int index)
        {
            CheckIndex(index);
            cellUpdateHandler?.Invoke(index);
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= cellCount)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}