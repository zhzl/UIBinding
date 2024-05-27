using System;
using UIBinding;
using UnityEngine;

namespace BasicUsage
{
    public abstract class UIView : IBindingProvider
    {
        private BindingView bindingView;

        public virtual void OnInitialize(Transform transform)
        {
            bindingView = new BindingView();
            bindingView.InitDatabinding(transform, this);
        }

        public virtual void OnOpen() { }

        public T GetViewModel<T>() where T : ViewModelBase
        {
            return bindingView.GetViewModel<T>();
        }

        public ViewModelBase CreateViewModel(string viewModelName)
        {
            return (ViewModelBase)Activator.CreateInstance(Type.GetType(viewModelName));
        }

        public void LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object
        {
            ResManager.LoadAssetAsync(path, onComplete);
        }
    }
}
