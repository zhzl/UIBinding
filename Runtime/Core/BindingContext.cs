using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 支持的上下文数据类型
    /// </summary>
    public enum EBindingData
    {
        PropertyName,
        ListViewCommandArgs,
        MaxCount,
    }

    /// <summary>
    /// 绑定上下文，一个ViewModel对应一个绑定上下文
    /// </summary>
    public class BindingContext
    {
        public ViewModelBase ViewModel => viewModel;

        private readonly BindingView bindingView;
        private readonly BindingConfig bindingConfig;
        private readonly object[] bindingDatas;

        private ViewModelBase viewModel;
        private BindingContext parentContext;

        public BindingContext(BindingView bindingView, BindingConfig bindingConfig, ViewModelBase viewModel)
        {
            this.viewModel = viewModel;
            this.bindingConfig = bindingConfig;
            this.bindingView = bindingView;
            this.bindingDatas = new object[(int)EBindingData.MaxCount];

            SyncProperties();
            viewModel.OnPropertyChanged += OnPropertyChanged;
        }

        public void SetParent(BindingContext parent)
        {
            this.parentContext = parent;
        }

        public void UpdateViewModel(ViewModelBase newViewModel)
        {
            viewModel.OnPropertyChanged -= OnPropertyChanged;
            newViewModel.OnPropertyChanged += OnPropertyChanged;
            viewModel = newViewModel;
        }

        public void UpdatePropertyValue(string propertyName, object propertyValue)
        {
            var propertyInfo = viewModel.GetType().GetProperty(propertyName);
            propertyInfo.SetValue(viewModel, propertyValue);
        }

        public void SetBindingData(EBindingData key, object value)
        {
            bindingDatas[(int)key] = value;
        }

        public object GetBindingData(EBindingData key)
        {
            var data = bindingDatas[(int)key];
            if (data == null && parentContext != null)
            {
                data = parentContext.GetBindingData(key);
            }
            return data;
        }

        public T GetBindingData<T>(EBindingData key)
        {
            return (T)GetBindingData(key);
        }

        #region 直接调用绑定视图的接口，属于同一个视图的 Context，不论从哪个调用，都会获得相同结果

        public BindingContext BindViewModel(Transform transform)
        {
            return bindingView.BindViewModel(transform, null);
        }

        public BindingContext BindViewModel(Transform transform, ViewModelBase viewModel)
        {
            return bindingView.BindViewModel(transform, viewModel);
        }

        public BindingContext GetContext(Transform transform)
        {
            return bindingView.GetContext(transform);
        }

        public ViewModelBase GetViewModel(Transform transform)
        {
            var context = bindingView.GetContext(transform);
            if (context == null)
            {
                return null;
            }    
            return context.ViewModel;
        }

        public void LoadAssetAsync<T>(string path, System.Action<T> onComplete) where T : UnityEngine.Object
        {
            bindingView.LoadAssetAsync<T>(path, onComplete);
        }

        #endregion

        private void SyncProperties()
        {
            var bindingCount = bindingConfig.itemList.Count;
            for (int i = 0; i < bindingCount; i++)
            {
                var bindingItem = bindingConfig.itemList[i];
                bindingItem.syncProperties = new List<ISyncProperty>();
                for (int j = 0; j < bindingItem.uiComponents.Count; j++)
                {
                    var uiProperty = bindingItem.uiProperties[j];
                    var syncProperty = PropertyFactory.Create(uiProperty);
                    bindingItem.syncProperties.Add(syncProperty);

                    if (syncProperty == null)
                    {
                        LogUtils.Error($"属性 {uiProperty} 未找到继承 ISyncProperty 的属性同步类");
                    }
                }
            }
        }

        private void OnPropertyChanged(object sender, int propertyIndex, string propertyName, object propertyValue)
        {
            var bindingItem = bindingConfig.itemList[propertyIndex];
            if (!bindingItem.vmPropertyName.Equals(propertyName, System.StringComparison.Ordinal))
            {
                LogUtils.Error($"属性名称不一致，{sender}[{propertyIndex}]({propertyName}), {bindingConfig}[{propertyIndex}]({bindingItem.vmPropertyName})");
                return;
            }

            SetBindingData(EBindingData.PropertyName, propertyName);

            for (int i = 0; i < bindingItem.syncProperties.Count; i++)
            {
                var syncProperty = bindingItem.syncProperties[i];
                syncProperty?.SyncProperty(bindingItem.uiComponents[i], propertyValue, this);
            }
        }
    }
}
