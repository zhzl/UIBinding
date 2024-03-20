using UnityEngine;

namespace UIBinding
{
    public class SyncSubViewModel : ISyncProperty
    {
        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            var newViewModel = vmPropertyValue as ViewModelBase;
            if (newViewModel == null)
            {
                return;
            }

            var transform = uiComponent as Transform;
            var oldViewModel = bindingContext.GetViewModel(transform);

            if (oldViewModel == null || newViewModel != oldViewModel)
            {
                var vmBindingContext = bindingContext.BindViewModel(transform, newViewModel);
                vmBindingContext.SetParent(bindingContext);
                newViewModel.NotifyAll();
            }
        }
    }
}
