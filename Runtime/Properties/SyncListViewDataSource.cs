using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    public class SyncListViewDataSource : ISyncProperty
    {
        private ListView listView;
        private BindingContext listContext;
        private IObservableList dataSource;
        private bool listend;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            listContext = bindingContext;
            listView = uiComponent as ListView;

            if (dataSource != null)
            {
                dataSource.CollectionChanged -= OnCollectionChanged;
            }

            dataSource = vmPropertyValue as IObservableList;
            dataSource.CollectionChanged += OnCollectionChanged;

            if (!listend)
            {
                listend = true;
                listView.OnCellUpdate += OnListCellUpdate;
            }

            listView.Refresh(dataSource.Count);
        }

        private void OnCollectionChanged(object sender, CollectionChangedAction changedAction, object changedItem, int changedIndex)
        {
            switch (changedAction)
            {
                case CollectionChangedAction.Add:
                case CollectionChangedAction.Remove:
                case CollectionChangedAction.Reset:
                    listView.Refresh(dataSource.Count);
                    break;
                case CollectionChangedAction.Update:
                    listView.RefreshIndex(changedIndex);
                    break;
                default:
                    break;
            }
        }

        private void OnListCellUpdate(int index)
        {
            var newCellViewModel = dataSource.Get(index) as ViewModelBase;
            if (newCellViewModel == null)
            {
                LogUtils.Error($"{dataSource}[{index}] is null.");
                return;
            }

            var cellTransform = listView.GetCell(index).transform;
            var oldCellViewModel = listContext.GetViewModel(cellTransform);
            if (oldCellViewModel == null || newCellViewModel != oldCellViewModel)
            {
                var cellContext = listContext.BindViewModel(cellTransform, newCellViewModel);
                cellContext.SetParent(listContext);

                var cellCommandArgs = cellContext.GetBindingData(EBindingData.ListViewCommandArgs) as ListViewCommandArgs;
                if (cellCommandArgs == null)
                {
                    cellCommandArgs = new ListViewCommandArgs();
                    cellContext.SetBindingData(EBindingData.ListViewCommandArgs, cellCommandArgs);
                }

                cellCommandArgs.index = index;
                cellCommandArgs.viewModel = newCellViewModel;

                newCellViewModel.NotifyAll();
            }
        }
    }
}
