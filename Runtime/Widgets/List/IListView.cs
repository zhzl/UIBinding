using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 列表通用接口
    /// </summary>
    public interface IListView
    {
        void AddCellUpdateListener(System.Action<int> cellUpdateCallback);
        void Init();
        void Refresh(int count);
        void RefreshIndex(int index);
        GameObject GetCell(int index);
    }
}
