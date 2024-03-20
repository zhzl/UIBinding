using UnityEngine;

namespace UIBinding
{
    public interface IBindingView
    {
        BindingContext GetContext(Transform transform);
        BindingContext BindViewModel(Transform transform, ViewModelBase viewModel);
        void LoadAssetAsync<T>(string path, System.Action<T> onComplete) where T : UnityEngine.Object;
    }
}
