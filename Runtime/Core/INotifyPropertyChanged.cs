namespace UIBinding
{
    /// <summary>
    /// 可观察对象接口
    /// </summary>
    public interface INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变事件
        /// </summary>
        event PropertyChangedEventHandler OnPropertyChanged;
    }
}
