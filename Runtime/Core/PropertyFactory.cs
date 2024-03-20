using System;
using System.Collections.Generic;

namespace UIBinding
{
    public static class PropertyFactory
    {
        private static readonly Dictionary<string, Type> propertyDic = new Dictionary<string, Type>();

        public static void Register<T>(string propertyName) where T : ISyncProperty
        {
            if (propertyDic.ContainsKey(propertyName))
            {
                LogUtils.Error($"重复注册属性：{propertyName}");
                return;
            }
            propertyDic[propertyName] = typeof(T);
        }

        public static void Register(string propertyName, Type syncType)
        {
            if (syncType == null)
            {
                LogUtils.Error("syncType is null");
                return;
            }
            if (propertyDic.ContainsKey(propertyName))
            {
                LogUtils.Error($"重复注册属性：{propertyName}");
                return;
            }
            if (!typeof(ISyncProperty).IsAssignableFrom(syncType))
            {
                LogUtils.Error($"{syncType} 必须集成自 ISyncProperty");
                return;
            }
            propertyDic[propertyName] = syncType;
        }

        public static ISyncProperty Create(string propertyName)
        {
            if (propertyDic.TryGetValue(propertyName, out var propertyType))
            {
                return (ISyncProperty)Activator.CreateInstance(propertyType);
            }

            return null;
        }

        public static void Reset()
        {
            propertyDic.Clear();
        }
    }
}
