using UnityEngine;

namespace UIBinding
{
    public class SyncLoadPrefabCommand : ISyncProperty
    {
        private LoadPrefabCommand command;
        private Transform parentTransform;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            command = vmPropertyValue as LoadPrefabCommand;
            parentTransform = uiComponent as Transform;

            if (string.IsNullOrEmpty(command.prefabPath))
            {
                LogUtils.Error($"未找到资源路径，请检查 LoadPrefabCommand 参数");
                return;
            }

            bindingContext.LoadAssetAsync<GameObject>(command.prefabPath, prefab =>
            {
                if (prefab == null)
                {
                    return;
                }

                var prefabObject = GameObject.Instantiate<GameObject>(prefab, parentTransform);
                var prefabTransform = prefabObject.transform;

                var prefabContext = bindingContext.BindViewModel(prefabTransform);
                prefabContext.SetParent(bindingContext);

                // prefab args
                var prefabCommandArgs = new PrefabLoadedCommandArgs();
                prefabCommandArgs.viewModel = prefabContext.ViewModel;

                // list args
                var bindingData = bindingContext.GetBindingData(EBindingData.ListViewCommandArgs);
                prefabCommandArgs.listViewArgs = bindingData as ListViewCommandArgs;

                command.Execute(prefabCommandArgs);
            });
        }
    }
}
