using System;

namespace UIBinding
{
    public interface IBindingProvider
    {
        T GetViewModel<T>() where T : ViewModelBase;
        ViewModelBase CreateViewModel(string viewModelName);
        void LoadAssetAsync<T>(string path, Action<T> onComplete) where T : UnityEngine.Object;
    }
}
