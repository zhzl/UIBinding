using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    /// <summary>
    /// 绑定视图基类
    /// </summary>
    public abstract class BindingViewBase : IBindingView
    {
        private Transform transform;
        private BindingContext viewContext;
        private Dictionary<Transform, BindingContext> transformContextMap;

        protected void InitDatabinding(Transform transform)
        {
            this.transform = transform;
            transformContextMap = new Dictionary<Transform, BindingContext>();
            viewContext = BindViewModel(transform, null);
        }

        protected abstract ViewModelBase CreateViewModel(string viewModelName);

        public abstract void LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object;

        public T GetViewModel<T>() where T : ViewModelBase
        {
            if (viewContext == null)
            {
                LogUtils.Error($"{transform} dont have viewmodel: {typeof(T).Name}");
                return null;
            }
            return viewContext.ViewModel as T;
        }

        public BindingContext GetContext(Transform transform)
        {
            transformContextMap.TryGetValue(transform, out var bindingContext);
            return bindingContext;
        }

        public BindingContext BindViewModel(Transform transform, ViewModelBase viewModel)
        {
            // 如果已绑定，直接刷新
            if (transformContextMap.TryGetValue(transform, out var bindingContext))
            {
                bindingContext.UpdateViewModel(viewModel);
                return bindingContext;
            }

            if (!transform.TryGetComponent<BindingConfig>(out var bindingConfig))
            {
                return null;
            }

            if (string.IsNullOrEmpty(bindingConfig.vmKlassName))
            {
                LogUtils.Error($"{transform}未指定绑定文件！");
                return null;
            }

            if (viewModel == null)
            {
                viewModel = CreateViewModel(bindingConfig.vmKlassName);
            }

            bindingContext = new BindingContext(this, bindingConfig, viewModel);
            transformContextMap[transform] = bindingContext;
            return bindingContext;
        }
    }
}
