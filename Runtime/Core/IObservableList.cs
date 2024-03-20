namespace UIBinding
{
    /// <summary>
    /// 可观察列表
    /// </summary>
    public interface IObservableList : INotifyCollectionChanged
    {
        /// <summary>
        /// 子项数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取指定下标的子项
        /// </summary>
        /// <param name="index">指定下标</param>
        object Get(int index);
    }
}
