using UnityEngine;

namespace UIBinding
{
    public delegate void OnCellUpdateHandler(int index);

    /// <summary>
    /// 通用列表组件，用于框架交互，此外需要有具体的列表实现类，该类继承自 IListView
    /// </summary>
    public class ListView : MonoBehaviour
    {
        public event OnCellUpdateHandler OnCellUpdate;

        private IListView listView;
        private bool isInited;

        /// <summary>
        /// 列表初始化
        /// </summary>
        public void Init()
        {
            if (isInited) return;
            isInited = true;

            listView = GetComponent<IListView>();
            listView.AddCellUpdateListener(OnListCellUpdate);
            listView.Init();
        }

        /// <summary>
        /// 刷新整个列表
        /// </summary>
        public void Refresh(int count)
        {
            Init();
            listView.Refresh(count);
        }

        /// <summary>
        /// 刷新指定下标的单元格
        /// </summary>
        /// <param name="index">指定下标</param>
        public void RefreshIndex(int index)
        {
            Init();
            listView.RefreshIndex(index);
        }

        /// <summary>
        /// 获取指定下标的cell对象
        /// </summary>
        /// <param name="index">指定下标</param>
        public GameObject GetCell(int index)
        {
            return listView.GetCell(index);
        }

        /// <summary>
        /// 列表刷新回调
        /// </summary>
        private void OnListCellUpdate(int index)
        {
            OnCellUpdate?.Invoke(index);
        }
    }
}
