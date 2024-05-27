using UIBinding;

namespace BasicUsage
{
    public class CustomSyncRegister : ISyncRegister
    {
        public void Register()
        {            
            // 反射注册
            var prFields = typeof(CustomPR).GetFields();
            for (int i = 0; i < prFields.Length; i++)
            {
                var field = prFields[i];
                if (!field.HasAttribute<PRBindAttribute>())
                    continue;

                PropertyFactory.Register(field.Name, System.Type.GetType($"BasicUsage.Sync{field.Name}"));
            }
        }
    }
}
