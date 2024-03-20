using UnityEngine;

namespace UIBinding
{
    public class SyncPropertyRegister : ISyncRegister
    {
        public void Register()
        {
            // None比较特殊，单独注册
            PropertyFactory.Register<SyncNone>(nameof(PR.None));

            // 反射注册其它属性
            var prFields = typeof(PR).GetFields();
            for (int i = 0; i < prFields.Length; i++)
            {
                var field = prFields[i];
                if (!field.HasAttribute<PRBindAttribute>())
                    continue;

                PropertyFactory.Register(field.Name, System.Type.GetType($"UIBinding.Sync{field.Name}"));
            }
        }
    }
}
