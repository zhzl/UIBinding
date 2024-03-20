namespace UIBinding
{
    /// <summary>
    /// 绑定初始化器
    /// </summary>
    public class BindingInitializer
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="customRegister">自定义属性同步注册器</param>
        /// <param name="customLogger">自定义日志</param>
        public static void Initialize(ISyncRegister customRegister = null, ILogger customLogger = null)
        {
            PropertyFactory.Reset();

            new SyncPropertyRegister().Register();
            if (customRegister != null)
            {
                customRegister.Register();
            }
            if (customLogger != null)
            {
                LogUtils.Logger = customLogger;
            }
        }
    }
}
