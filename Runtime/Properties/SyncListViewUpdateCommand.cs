using System.Collections.Generic;
using UnityEngine;

namespace UIBinding
{
    public class SyncListViewUpdateCommand : ISyncProperty
    {
        private ListView listView;
        private BindingContext listContext;
        private ICommand command;
        private bool listened;

        public void SyncProperty(Component uiComponent, object vmPropertyValue, BindingContext bindingContext)
        {
            listContext = bindingContext;
            command = vmPropertyValue as SimpleCommand;

            if (!listened)
            {
                listened = true;
                listView = uiComponent as ListView;
                listView.OnCellUpdate += OnListCellUpdate;
            }
        }

        private void OnListCellUpdate(int index)
        {
            var cellTransform = listView.GetCell(index).transform;
            var cellContext = listContext.GetContext(cellTransform);
            if (cellContext == null)
            {
                cellContext = listContext.BindViewModel(cellTransform);
                cellContext.SetBindingData(EBindingData.ListViewCommandArgs, new ListViewCommandArgs());
                cellContext.SetParent(listContext);
            }

            var cellCommandArgs = cellContext.GetBindingData(EBindingData.ListViewCommandArgs) as ListViewCommandArgs;
            cellCommandArgs.index = index;
            cellCommandArgs.viewModel = cellContext.ViewModel;
            command?.Execute(cellCommandArgs);
        }
    }
}
