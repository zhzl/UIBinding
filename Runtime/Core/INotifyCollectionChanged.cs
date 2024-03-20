namespace UIBinding
{
    /// <summary>
    /// 可观察集合接口
    /// </summary>
    public interface INotifyCollectionChanged
    {
        /// <summary>
        /// 集合改变事件
        /// </summary>
        event CollectionChangedEventHandler CollectionChanged;
    }
}
