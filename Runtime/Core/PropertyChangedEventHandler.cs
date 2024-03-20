namespace UIBinding
{
    /// <summary>
    /// ViewModel 属性改变代理
    /// </summary>
    /// <param name="viewModel">改变属性的 ViewModel</param>
    /// <param name="propertyIndex">改变的属性下标</param>
    /// <param name="propertyName">改变的属性名字</param>
    /// <param name="propertyValue">新的属性值</param>
    public delegate void PropertyChangedEventHandler(object viewModel, int propertyIndex, string propertyName, object propertyValue);
}
