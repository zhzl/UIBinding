namespace UIBinding
{
    /// <summary>
    /// 集合子项改变代理
    /// </summary>
    /// <param name="sender">改变子项的集合</param>
    /// <param name="changedAction">改变子项的操作</param>
    /// <param name="changedItem">改变的子项对象</param>
    /// <param name="changedIndex">改变的子项下表</param>
    public delegate void CollectionChangedEventHandler(object sender, CollectionChangedAction changedAction, object changedItem = null, int changedIndex = -1);
}
