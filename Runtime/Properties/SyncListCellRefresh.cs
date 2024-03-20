using UnityEngine;

namespace UIBinding
{
    public class SyncListCellRefresh : ISyncProperty
    {
        private ListView listView;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            listView = uiComponent as ListView;
            var refreshIndex = (vmPropertyValue as Boxed<int>).value;
            listView.RefreshIndex(refreshIndex);
        }
    }
}
