using UnityEngine;

namespace UIBinding
{
    public class SyncListViewCount : ISyncProperty
    {
        private ListView listView;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            listView = uiComponent as ListView;
            var cellCount = (vmPropertyValue as Boxed<int>).value;
            listView.Refresh(cellCount);
        }
    }
}
